using System;
using System.Collections.Generic;
using System.Linq;

namespace GodnessChatBot.Domain.LearningWays
{
    public class LearningWayByTest : LearningWay
    {
        public override string Name => "Тест";
        private LearningByTestState state = LearningByTestState.WaitingQuestion;
        public LearningWayByTest() => NeedNextCard = true;

        public override ReplyMessage Learn(Card card, Pack pack, string message)
        {
            if (state == LearningByTestState.WaitingQuestion)
            {
                var question = card.Face;
            
                var random = new Random();
                var possibleAnswers = pack.Cards
                    .Where(c => !Equals(c, card))
                    .OrderBy(x => random.Next())
                    .Select(c => c.Back)
                    .Take(3)
                    .Append(card.Back)
                    .OrderBy(x => random.Next())
                    .ToList();

                state = LearningByTestState.WaitingResult;

                return new ReplyMessage(new List<string>{question}, possibleAnswers);
            }

            var result = string.Equals(card.Back, message, StringComparison.CurrentCultureIgnoreCase);
            var answer = result
                ? "Верно!"
                : $"Неверно :(\nПравильный ответ: {card.Back}";
            
            CalculateStatistic(card, result);
            state = LearningByTestState.WaitingQuestion;
            return new ReplyMessage(new List<string>{answer});
        }
    }
    
    public enum LearningByTestState
    {
        WaitingQuestion,
        WaitingResult
    }
}