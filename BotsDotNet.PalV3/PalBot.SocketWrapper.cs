using Newtonsoft.Json;
using SocketIOClient;
using SocketIOClient.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotsDotNet.PalringoV3
{
    using Delegates;
    using Models;
    using Network;

    public partial class PalBot
    {
        public event VoidCarrier OnConnected = delegate { };
        public event VoidCarrier OnDisconnected = delegate { };
        public event VoidCarrier OnReconnectFailed = delegate { };

        public static string ServerUrl = "https://v3.palringo.com:3051";

        public string Token { get; private set; }

        public Task<T> WritePacket<T>(Packet packet)
        {
            var tsc = new TaskCompletionSource<T>();
            Connection.EmitAsync(packet.Command,
                new
                {
                    headers = packet?.Headers,
                    body = packet?.Body
                },
                (res) => HandleCallBack(res, tsc, packet.Command));
            return tsc.Task;
        }

        public Task<object> WritePacket(Packet packet)
        {
            return WritePacket<object>(packet);
        }

        public Task<T> On<T>(string evt)
        {
            var task = new TaskCompletionSource<T>();

            Connection.On(evt, (data) =>
            {
                if (task.Task.IsCompleted)
                    return;

                var d = JsonConvert.DeserializeObject<T>(data.Text);
                task.SetResult(d);
            });

            return task.Task;
        }
       
        public void On<T>(string evt, Action<T> action, Func<ResponseArgs, T> func = null)
        {
            Func<ResponseArgs, T> def = (r) => DataTransform<T>(JsonConvert.DeserializeObject(r.Text), evt: evt);
            func = func ?? def;

            Connection.On(evt, (msg) =>
            {
                try
                {
                    var type = func(msg);
                    action(type);
                }
                catch (Exception ex)
                {
                    Error(ex);
                }
            });
        }

        public void MessageOn(Action<BaseMessage> action)
        {
            On("message send", action, (r) =>
            {
                try
                {
                    var pk = JsonConvert.DeserializeObject<Packet<BaseMessage>>(r.Text);

                    var msg = pk.Body;
                    if (msg.Data.Placeholder)
                    {
                        var data = r.Buffers[msg.Data.Num];
                        var stuff = System.Text.Encoding.UTF8.GetString(data.ToArray());
                        msg.Contents = stuff;
                    }

                    return msg;
                }
                catch (Exception ex)
                {
                    Error(ex);
                    return new BaseMessage();
                }
            });
        }

        public void GroupMemberUpdate()
		{
            On("group member update", (GroupUserUpdate update) =>
            {
                cacheUtility.UpdateGroupMember(update, false);
            });

            On("group member delete", (GroupUserUpdate update) =>
            {
                cacheUtility.UpdateGroupMember(update, true);
            });

            On("group member add", (GroupUserUpdate update) =>
            {
                cacheUtility.UpdateGroupMember(update, false);
            });
		}

        private void HandleCallBack<T>(ResponseArgs result, TaskCompletionSource<T> task, string evt = "")
        {
            try
            {
                var res = DataTransform<T>(JsonConvert.DeserializeObject(result.Text), evt: evt);

                task.SetResult(res);
            }
            catch (Exception ex)
            {
                task.SetException(new SocketException(result, "No fitting result set found within socket.", ex));
            }
        }

        private T DataTransform<T>(object result, bool skipCodeCheck = false, string evt = "")
        {
            dynamic codedDynamic = result;
            int code = codedDynamic.code;

            if (code > 299 && code < 200 && !skipCodeCheck)
            {
                throw new SocketException(result, "Error during translation");
            }

            if (typeof(T) == typeof(Resp))
            {
                return (T)(object)new Resp
                {
                    Code = code,
                    Raw = JsonConvert.SerializeObject(result)
                };
            }

            object body = codedDynamic.body;

            if (typeof(T) == typeof(object))
            {
                return (T)body;
            }

            var data = JsonConvert.SerializeObject(body);
            var res = JsonConvert.DeserializeObject<T>(data);

            return res;
        }

        private SocketIO CreateSocket(string token = null)
        {
            Token = token ?? this.GenerateDeviceToken();
            var socket = new SocketIO(ServerUrl)
            {
                Parameters = new Dictionary<string, string>
                {
                    {"token", Token },
                    {"device", "web" }
                }
            };

            socket.OnConnected += () => OnConnected();
            socket.OnClosed += Socket_OnClosed;
            socket.OnError += (e) => Error(new SocketException(e.RawText, "Error occured in socket: " + e.Text));

            socket.UnhandledEvent += Socket_OnReceivedEvent;
            socket.OnReceivedEvent += Socket_OnReceivedEvent;
            //socket.

            return socket;
        }

        private async void Socket_OnClosed(ServerCloseReason obj)
        {
            OnDisconnected();
            if (obj == ServerCloseReason.Aborted ||
                obj == ServerCloseReason.ClosedByClient ||
                !AutoReconnect)
                return;

            var result = await Login(Email, Password, Token, Prefix);
            //var wt = On<Welcome>("welcome");

            if (!result)
                OnReconnectFailed();
        }

        private void Socket_OnReceivedEvent(string arg1, ResponseArgs arg2)
        {
            //Console.WriteLine("Socket Recieved: " + arg1 + arg2.Text);
        }

        public void WriteObject(object data, string prefix = "")
        {
            Console.WriteLine(prefix + JsonConvert.SerializeObject(data, Formatting.Indented));
        }
    }
}
