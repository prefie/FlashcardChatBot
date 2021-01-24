using System;
using System.Collections.Generic;

namespace GodnessChatBot.Domain.LearningWays
{
    public class LearningWayByTyping : LearningWay
    {
        public override string Name => "Ввод ответа";
        private LearningByTypingState state = LearningByTypingState.WaitingQuestion;

        public LearningWayByTyping() => NeedNextCard = true;
        
        public override ReplyMessage Learn(Card card, Pack pack, string message)
        {
            if (state == LearningByTypingState.WaitingQuestion)
            {
                var question = card.Face;
                state = LearningByTypingState.WaitingResult;
                return new ReplyMessage(new List<string> {question});
            }

            var result = string.Equals(card.Back, message, StringComparison.CurrentCultureIgnoreCase);
            var answer = result
                ? "Верно!"
                : $"Неверно :(\nПравильный ответ: {card.Back}";
            
            CalculateStatistic(card, result);
            state = LearningByTypingState.WaitingQuestion;
            return new ReplyMessage(new List<string> {answer});
        }
    }

    public enum LearningByTypingState
    {
        WaitingQuestion,
        WaitingResult
    }
}