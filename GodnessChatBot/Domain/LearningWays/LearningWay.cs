using System.Collections.Generic;

namespace GodnessChatBot.Domain.LearningWays
{
    public abstract class LearningWay
    {
        public abstract string Name { get; }
        public abstract ReplyMessage Learn(Card card, Pack pack, string message);
        public bool NeedNextCard { get; protected set; }
        
        protected void CalculateStatistic(Card card, bool result)
        {
            if (result)
                card.Statistic++;
            else
                card.Statistic--;
        }
    }
}