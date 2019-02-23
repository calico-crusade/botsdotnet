using System;
using Newtonsoft.Json;

namespace BotsDotNet.WebExTeams.SparkDotNet
{
    public class SparkMessage
    {
        public string Id { get; set; }
        public string RoomId { get; set; }
        public string RoomType { get; set; }
        public string ToPersonId { get; set; }
        public string ToPersonEmail { get; set; }
        public string Text { get; set; }
        public string Markdown { get; set; }
        public string[] Files { get; set; }
        public string PersonId { get; set; }
        public string PersonEmail { get; set; }
        public string Html { get; set; }
        public DateTime Created { get; set; }
        public string[] MentionedPeople { get; set; }


        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}