using System;
using System.Collections.Generic;
using System.Linq;

namespace GodnessChatBot
{
    public class Teacher
    {
        private readonly List<Category> categories;
        public IReadOnlyList<Category> Categories => categories.AsReadOnly();
        public TeacherStatus Status { get; private set; }
        private Pack CurrentPack;
        private int CurrentIndex;
        private LearningWay CurrentLearningWay;

        public Teacher(List<Category> categories)
        {
            this.categories = categories;
            Status = TeacherStatus.Idles;
        }
        
        public Category GetCategory(string name)
        {
            foreach (var category in categories.Where(pack => pack.Name == name))
                return category;

            throw new ArgumentException($"The {name} category does not exist.");
        }

        public void StartLearning(string nameCategory, string namePack, LearningWay learningWay)
        {
            if (Status != TeacherStatus.Idles) throw new InvalidOperationException();
            
            CurrentPack = GetCategory(nameCategory).GetPack(namePack);
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
            return CurrentPack[CurrentIndex].Face;
        }

        public List<string> GetPossibleAnswers()
        {
            if (Status != TeacherStatus.WaitingAnswer) throw new InvalidOperationException();
            
            var random = new Random();
            Status = TeacherStatus.ReceivingResult;
            switch (CurrentLearningWay)
            {
                case LearningWay.LearnByTyping:
                    return new List<string>();
                case LearningWay.LearnYourself:
                    return new List<string> {"Показать ответ"};
                case LearningWay.LearnByTest:
                {
                    return CurrentPack.Cards
                        .Where(card => !Equals(card, CurrentPack[CurrentIndex]))
                        .OrderBy(x => random.Next())
                        .Select(card => card.Back)
                        .Take(3)
                        .Append(CurrentPack[CurrentIndex].Back)
                        .OrderBy(x => random.Next())
                        .ToList();
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public bool GiveAnswer(string answer)
        {
            if (Status != TeacherStatus.ReceivingResult) throw new InvalidOperationException();

            Status = TeacherStatus.ReceivingBackCard;
            return CurrentPack[CurrentIndex].Back == answer;
        }

        public string GetBackCard()
        {
            if (Status != TeacherStatus.ReceivingBackCard) throw new InvalidOperationException();
            
            CurrentIndex += 1;
            
            Status = CurrentIndex == CurrentPack.Cards.Count ? TeacherStatus.Idles : TeacherStatus.ReceivingFaceСard;
                
            return CurrentPack[CurrentIndex - 1].Back;
        }
    }
}