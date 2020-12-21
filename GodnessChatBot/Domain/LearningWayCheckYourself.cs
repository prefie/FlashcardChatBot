using System.Collections.Generic;

namespace GodnessChatBot
{
    public class LearningWayCheckYourself : ILearningWay
    {
        public string Name { get; set; }
        public Pack Pack { get; set; }
        private int CardIndex { get; set; }
        
        public LearningWayCheckYourself()
        {
        }
        
        public LearningWayCheckYourself(Pack pack)
        {
            Pack = pack;
        }
        
        public string SendQuestion(int cardIndex)
        {
            CardIndex = cardIndex;
            return Pack[CardIndex].Face;
        }

        public List<string> SendPossibleAnswers() => new List<string> { "Помню", "Не помню" };

        public string GetAnswer(string answer)
        {
            throw new System.NotImplementedException();
        }
    }
}