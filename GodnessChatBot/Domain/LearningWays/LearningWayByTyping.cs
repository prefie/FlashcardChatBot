using System;
using System.Collections.Generic;

namespace GodnessChatBot.Domain.LearningWays
{
    public class LearningWayByTyping : ILearningWay
    {
        public string Name => "Ввод ответа";
        public Pack Pack { get; set; }
        private int CardIndex { get; set; }
        
        public LearningWayByTyping() {}
        
        public LearningWayByTyping(Pack pack)
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
            return new List<string>();
        }

        public bool? GetAnswer(out string answer, string message)
        {
            answer = string.Equals(Pack[CardIndex].Back, message, StringComparison.CurrentCultureIgnoreCase)
                ? "Верно!"
                : $"Неверно :(\nПравильный ответ: {Pack[CardIndex].Back}";
            return string.Equals(Pack[CardIndex].Back, message, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}