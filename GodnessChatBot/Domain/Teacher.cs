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
            Status = TeacherStatus.Idles;
        }

        public void StartLearning(string namePack, ILearningWay learningWay)
        {
            if (Status != TeacherStatus.Idles) throw new InvalidOperationException();
            
            if(!CurrentPack.CanLearningWay(learningWay))
                throw new ArgumentException($"The deck does not support {learningWay} learning method.");
            
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

            return CurrentLearningWay.SendPossibleAnswers();
        }

        public void MakeStatisticByAnswerResult(string answer)
        {
            if (Status != TeacherStatus.ReceivingBackCard) throw new InvalidOperationException();

            if (CurrentLearningWay.GetAnswer(answer) == "Верно!")
                CurrentPack[CurrentIndex].Statistic++;
            else 
                CurrentPack[CurrentIndex].Statistic--;
            
            CurrentIndex = CurrentPack.Cards.OrderBy(x => x.Statistic)
                .Where(x => x.Statistic < 10)
                .Select(x => x.Statistic)
                .FirstOrDefault();
            
            Status = TeacherStatus.ReceivingFaceСard;
        }
    }
}