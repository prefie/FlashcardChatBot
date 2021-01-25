using System;
using System.Collections.Generic;
using System.Linq;

namespace GodnessChatBot.Domain.LearningWays
{
    public class LearningWayByTest : LearningWay
    {
        public override string Name => "Тест";
        private LearningByTestState state = LearningByTestState.WaitingFirstCard;
        public LearningWayByTest() => NeedNextCard = true;

        public override ReplyMessage Learn(Card previousCard, Card card, Pack pack, string message)
        {
            if (state == LearningByTestState.WaitingFirstCard)
            {
                var question = card.Face;
                var possibleAnswers = GetPossibleAnswers(card, pack);
                state = LearningByTestState.WaitingAnotherCard;

                return new ReplyMessage(new List<string>{question}, possibleAnswers);
            }

            var result = string.Equals(previousCard.Back, message, StringComparison.CurrentCultureIgnoreCase);
            var answer = result
                ? "Верно!"
                : $"Неверно :(\nПравильный ответ: {previousCard.Back}";
            
            CalculateStatistic(previousCard, result);
            var nextPossibleAnswers = GetPossibleAnswers(card, pack);
            return new ReplyMessage(new List<string>{answer, card.Face}, nextPossibleAnswers);
        }
        
        private List<string> GetPossibleAnswers(Card card, Pack pack)
        {
            var random = new Random();
            var possibleAnswers = pack.Cards
                .Where(c => !string.Equals(c.Back, card.Back, StringComparison.CurrentCultureIgnoreCase))
                .OrderBy(x => random.Next())
                .Select(c => c.Back)
                .Distinct(StringComparer.CurrentCultureIgnoreCase)
                .Take(3)
                .Append(card.Back)
                .OrderBy(x => random.Next())
                .ToList();
            return possibleAnswers;
        }
    }
    
    public enum LearningByTestState
    {
        WaitingFirstCard,
        WaitingAnotherCard
    }
}