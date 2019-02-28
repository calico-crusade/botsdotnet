using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Quobject.SocketIoClientDotNet.Client;

namespace BotsDotNet.PalringoV3
{
    using Delegates;
    using Network;

    public partial class PalBot
    {
        public event VoidCarrier OnConnected = delegate { };
        public event VoidCarrier OnDisconnected = delegate { };

        public static string PalringoServerUri = "https://v3.palringo.com:3051";

        public string Token { get; private set; }

        public Task<T> WritePacket<T>(Packet packet)
        {
            var tsc = new TaskCompletionSource<T>();
            Connection.Emit(packet.Command, 
                (res) => HandleCallBack(res, tsc, packet.Command), 
                JObject.FromObject(new
                {
                    body = packet.Body
                }));
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

                var ser = JsonConvert.SerializeObject(data);
                var d = JsonConvert.DeserializeObject<T>(ser);
                task.SetResult(d);
            });

            return task.Task;
        }

        public void On<T>(string evt, Action<T> action)
        {
            Connection.On(evt, (msg) =>
            {
                try
                {
                    var type = DataTransform<T>(msg, evt: evt);
                    action(type);
                }
                catch (Exception ex)
                {
                    Error(ex);
                }
            });
        }

        private void HandleCallBack<T>(object result, TaskCompletionSource<T> task, string evt = "")
        {
            try
            {
                var res = DataTransform<T>(result, evt: evt);

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

            object body = codedDynamic.body;

            if (typeof(T) == typeof(object))
            {
                return (T)body;
            }

            var data = JsonConvert.SerializeObject(body);
            var res = JsonConvert.DeserializeObject<T>(data);

            return res;
        }

        private Socket CreateSocket(string token = null)
        {
            Token = token ?? this.GenerateDeviceToken();
            var socket = IO.Socket(PalringoServerUri, new IO.Options
            {
                Reconnection = true,
                Transports = new[] { "websocket" }.ToImmutable(),
                Query = new Dictionary<string, string>
                {
                    ["token"] = Token,
                    ["device"] = "web"
                }
            });

            socket.On(Socket.EVENT_CONNECT, () => OnConnected());
            socket.On(Socket.EVENT_DISCONNECT, () => OnDisconnected());
            socket.On(Socket.EVENT_ERROR, (e) => Error(new SocketException(e, "Error occurred in socket.")));
            socket.On(Socket.EVENT_CONNECT_ERROR, (e) => Error(new SocketException(e, "Error occurred during socket connection.")));
            
            return socket;
        }

        private void WriteObject(object data, string prefix = "")
        {
            Console.WriteLine(prefix + JsonConvert.SerializeObject(data, Formatting.Indented));
        }
    }
}
