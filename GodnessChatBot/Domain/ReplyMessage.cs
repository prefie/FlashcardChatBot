using System.Collections.Generic;

namespace GodnessChatBot.Domain
{
    public class ReplyMessage
    {
        public string Messages { get; }
        public List<string> ReplyOptions { get; }

        public ReplyMessage(List<string> messages, List<string> replyOptions)
        {
            Messages = string.Join("\n\n", messages);
            ReplyOptions = replyOptions;
        }

        public ReplyMessage(List<string> messages)
        {
            Messages = string.Join("\n\n", messages);
            ReplyOptions = new List<string>();
        }
    }
}