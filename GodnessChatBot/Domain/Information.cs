using System.Collections.Generic;

namespace GodnessChatBot
{
    public class Information
    {
        public List<string> Messages { get; }
        public List<string> AdditionalInfo { get; }

        public Information(List<string> messages, List<string> additionalInfo)
        {
            Messages = messages;
            AdditionalInfo = additionalInfo;
        }

        public Information(List<string> messages)
        {
            Messages = messages;
            AdditionalInfo = new List<string>();
        }
    }
}