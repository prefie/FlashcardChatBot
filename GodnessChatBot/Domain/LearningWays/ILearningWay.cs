using System.Collections.Generic;

namespace GodnessChatBot.Domain.LearningWays
{
    public interface ILearningWay
    {
        string Name { get; }
        Pack Pack { get; set; }
        string SendQuestion(int cardIndex);
        List<string> SendPossibleAnswers();
        bool? GetAnswer(out string answer, string message);
    }
}