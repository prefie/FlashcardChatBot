using System.Collections.Generic;

namespace GodnessChatBot
{
    public class LearningWayByTyping : ILearningWay
    {
        public string Name { get; set; }
        public Pack Pack { get; set; }
        private int CardIndex { get; set; }
        
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
            throw new System.NotImplementedException();
        }

        public string GetAnswer(string answer)
            => Pack[CardIndex].Back == answer 
                ? "Верно!" 
                : $"Неверно :(\nПравильный ответ: {Pack[CardIndex].Back}";
    }
}