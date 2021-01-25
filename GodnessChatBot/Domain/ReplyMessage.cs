using System.Collections.Generic;
using System.Linq;

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

        public ReplyMessage JoinReplyMessages(ReplyMessage otherMessage) =>
            new ReplyMessage(new List<string>{Messages, otherMessage.Messages},
                ReplyOptions.Concat(otherMessage.ReplyOptions).ToList());
    }
}