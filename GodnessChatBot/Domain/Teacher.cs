using System;
using System.Collections.Generic;
using System.Linq;

namespace GodnessChatBot
{
    public class Teacher
    {
        public TeacherStatus Status { get; private set; }
        private Pack CurrentPack;
        private int CurrentIndex;
        private ILearningWay CurrentLearningWay;

        public Teacher(Pack pack)
        {
            CurrentPack = pack;
            CurrentPack.OrderCards();
            Status = TeacherStatus.Idles;
        }

        public void StartLearning(ILearningWay learningWay)
        {
            if (Status != TeacherStatus.Idles) throw new InvalidOperationException();

            Status = TeacherStatus.ReceivingFaceСard;
            CurrentIndex = 0;
            CurrentLearningWay = learningWay;
        }

        public string GetFaceCard()
        {
            if (Status != TeacherStatus.ReceivingFaceСard) throw new InvalidOperationException();

            Status = TeacherStatus.WaitingAnswer;
            return CurrentLearningWay.SendQuestion(CurrentIndex);
        }

        public List<string> GetPossibleAnswers()
        {
            if (Status != TeacherStatus.WaitingAnswer) throw new InvalidOperationException();
            Status = TeacherStatus.ReceivingBackCard;

            return CurrentLearningWay.SendPossibleAnswers();
        }

        public string MakeStatisticByAnswerResult(string message)
        {
            if (Status != TeacherStatus.ReceivingBackCard) throw new InvalidOperationException();

            if (CurrentLearningWay.GetAnswer(out var answer, message))
                CurrentPack[CurrentIndex].Statistic++;
            else 
                CurrentPack[CurrentIndex].Statistic--;

            CurrentIndex = (CurrentIndex + 1) % CurrentPack.Cards.Count; 
            
            Status = TeacherStatus.ReceivingFaceСard;
            return answer;
        }
    }
}