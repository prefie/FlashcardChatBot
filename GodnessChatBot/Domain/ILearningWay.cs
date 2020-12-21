﻿using System.Collections.Generic;

namespace GodnessChatBot
{
    public interface ILearningWay
    {
        string Name { get; set; }
        Pack Pack { get; set; }
        string SendQuestion(int cardIndex);
        List<string> SendPossibleAnswers();
        string GetAnswer(string answer);
    }
}