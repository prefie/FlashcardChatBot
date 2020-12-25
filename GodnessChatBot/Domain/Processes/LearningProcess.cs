using System;
using System.Collections.Generic;

namespace GodnessChatBot
{
    public class LearningProcess : IProcess
    {
        private LearningEnum status = LearningEnum.Idles;
        private Pack pack;
        private ILearningWay learningWay;
        private int CurrentIndex;
        
        public Telegramma Start(string id, object obj)
        {
            if (status != LearningEnum.Idles || status != LearningEnum.Start)
                throw new InvalidOperationException();
            
            if (status == LearningEnum.Idles)
            {
                pack = Repository.GetPack(id, obj.ToString());

                if (pack == null)
                    return new Telegramma(new List<string> { "Этой колоды нет или она пустая, давай создадим ее!"});
                
                pack.OrderCards();
                status = LearningEnum.Start;
                return new Telegramma(new List<string> { "Выбери способ обучения" });
            }
            
            learningWay = (ILearningWay) obj;
            learningWay.Pack = pack;
            CurrentIndex = 0;
            var question = learningWay.SendQuestion(CurrentIndex);
            var answers = learningWay.SendPossibleAnswers();
            status = LearningEnum.Execute;
            
            return new Telegramma(new List<string> {question},answers);
        }

        public Telegramma Execute(string id, string message)
        {
            if(status == LearningEnum.Execute)
            {
                if (learningWay.GetAnswer(out var answer, message))
                    learningWay.Pack[CurrentIndex].Statistic++;
                else
                    learningWay.Pack[CurrentIndex].Statistic--;
                
                CurrentIndex = (CurrentIndex + 1) % learningWay.Pack.Cards.Count;
                
                var question = learningWay.SendQuestion(CurrentIndex);
                var answers = learningWay.SendPossibleAnswers();
                
                return new Telegramma(new List<string> {answer, question},answers);
            }
            
            throw new InvalidOperationException();
        }

        public Telegramma Finish(string id)
        {
            Repository.UpdateStatisticsPack(id, learningWay.Pack);
            return new Telegramma(new List<string> { "Отличная тренировка!" });
        }
    }
    
    public enum LearningEnum
    {
        Idles,
        Start,
        Execute
    }
}