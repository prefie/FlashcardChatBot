using System.Collections.Generic;

namespace GodnessChatBot
{
    public class Telegramma
    {
        public List<string> Messages { get; }
        public List<string> AdditionalInfo { get; }

        public Telegramma(List<string> messages, List<string> additionalInfo)
        {
            Messages = messages;
            AdditionalInfo = additionalInfo;
        }

        public Telegramma(List<string> messages)
        {
            Messages = messages;
            AdditionalInfo = new List<string>();
        }
    }
}