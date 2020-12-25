using System;
using System.Collections.Generic;
using System.Linq;

namespace GodnessChatBot
{
    public class LearningWayByTest : ILearningWay
    {
        public string Name => "Тест";
        public Pack Pack { get; set; }
        private int CardIndex { get; set; }
        
        public LearningWayByTest() {}

        public LearningWayByTest(Pack pack)
        {
            Pack = pack;
        }

        public string SendQuestion(int cardIndex)
        {
            CardIndex = cardIndex;
            return Pack[CardIndex].Face;
        }

        public List<string> SendPossibleAnswers()
        {
            var random = new Random();
            return Pack.Cards
                .Where(card => !Equals(card, Pack[CardIndex]))
                .OrderBy(x => random.Next())
                .Select(card => card.Back)
                .Take(3)
                .Append(Pack[CardIndex].Back)
                .OrderBy(x => random.Next())
                .ToList();
        }

        public bool GetAnswer(out string answer, string message)
        {
             answer = String.Equals(Pack[CardIndex].Back, message, StringComparison.CurrentCultureIgnoreCase)
                ? "Верно!"
                : $"Неверно :(\nПравильный ответ: {Pack[CardIndex].Back}";
             return String.Equals(Pack[CardIndex].Back, message, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}