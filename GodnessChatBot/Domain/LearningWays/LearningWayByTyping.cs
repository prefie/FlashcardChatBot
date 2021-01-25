using System;
using System.Collections.Generic;

namespace GodnessChatBot.Domain.LearningWays
{
    public class LearningWayByTyping : LearningWay
    {
        public override string Name => "Ввод ответа";
        private LearningByTypingState state = LearningByTypingState.WaitingFirstCard;

        public LearningWayByTyping() => NeedNextCard = true;
        
        public override ReplyMessage Learn(Card previousCard, Card card, Pack pack, string message)
        {
            if (state == LearningByTypingState.WaitingFirstCard)
            {
                var question = card.Face;
                state = LearningByTypingState.WaitingAnotherCard;
                return new ReplyMessage(new List<string> {question});
            }

            var result = string.Equals(previousCard.Back, message, StringComparison.CurrentCultureIgnoreCase);
            var answer = result
                ? "Верно!"
                : $"Неверно :(\nПравильный ответ: {previousCard.Back}";
            
            CalculateStatistic(previousCard, result);
            return new ReplyMessage(new List<string> {answer, card.Face});
        }
    }

    public enum LearningByTypingState
    {
        WaitingFirstCard,
        WaitingAnotherCard
    }
}