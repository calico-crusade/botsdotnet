using RedditSharp.Things;
using System;

namespace BotsDotNet.Reddit
{
    public class MessageObserver : IObserver<Comment>
    {
        private RedditBot bot;
        public MessageObserver(RedditBot bot)
        {
            this.bot = bot;
        }

        public void OnCompleted()
        {
            Console.WriteLine("Completed");
        }

        public void OnError(Exception error)
        {
            bot.Error(error);
        }

        public async void OnNext(Comment value)
        {
            var message = new Message(bot, value);
            Console.WriteLine("Found: " + message.User.Nickname + ": " + message.Content);
            await bot.MessageReceived(message);
        }
    }
}
