namespace GodnessChatBot
{
    public class LearningWayByTest : ILearningWay
    {
        public string Name { get; set; }
        public Pack Pack { get; set; }

        public void SendQuestion()
        {
            throw new System.NotImplementedException();
        }

        public void SendPossibleAnswers()
        {
            throw new System.NotImplementedException();
        }

        public void GetAnswer(string answer)
        {
            throw new System.NotImplementedException();
        }
    }
}