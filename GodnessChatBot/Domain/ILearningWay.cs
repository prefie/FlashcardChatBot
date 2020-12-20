namespace GodnessChatBot
{
    public interface ILearningWay
    {
        string Name { get; set; }
        Pack Pack { get; set; }
        void SendQuestion();
        void SendPossibleAnswers();
        void GetAnswer(string answer);
    }
}