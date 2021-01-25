namespace GodnessChatBot.Domain.LearningWays
{
    public abstract class LearningWay
    {
        public abstract string Name { get; }
        public abstract ReplyMessage Learn(Card previousCard, Card card, Pack pack, string message);
        public bool NeedNextCard { get; protected set; }
        
        protected void CalculateStatistic(Card card, bool result)
        {
            if (result)
            {
                if (card.Statistic < 10)
                    card.Statistic++;
            }
            else
            {
                if (card.Statistic > -10)
                    card.Statistic--;
            }
        }
    }
}