using System;
using System.Collections.Generic;

namespace GodnessChatBot.Domain.LearningWays
{
    public class LearningWayCheckYourself : ILearningWay
    {
        public string Name => "Самопроверка";
        public Pack Pack { get; set; }
        private int CardIndex { get; set; }
        private bool isRepeatedQuestion;
        
        public LearningWayCheckYourself() {}
        
        public LearningWayCheckYourself(Pack pack)
        {
            Pack = pack;
        }
        
        public string SendQuestion(int cardIndex)
        {
            CardIndex = cardIndex;
            return !isRepeatedQuestion ? Pack[CardIndex].Face : Pack[CardIndex].Back;
        }

        public List<string> SendPossibleAnswers()
        {
            if (isRepeatedQuestion)
            {
                isRepeatedQuestion = false;
                return new List<string> {"Помню", "Не помню"};
            }
            
            return new List<string> {"Показать ответ", "Помню", "Не помню"};
        }

        public bool? GetAnswer(out string answer, string message)
        {
            answer = null;
            if (string.Equals(message, "Показать ответ", StringComparison.InvariantCultureIgnoreCase))
            {
                isRepeatedQuestion = true;
                return null;
            }

            answer = "Хорошо, идём дальше";
            return string.Equals(message, "Помню", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}