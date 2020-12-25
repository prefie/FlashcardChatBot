using System;
using System.Collections.Generic;
using System.Linq;

namespace GodnessChatBot
{
    public class LearningProcess : IProcess
    {
        private LearningEnum status = LearningEnum.Idles;
        private HashSet<ILearningWay> learningWays;
        
        private Pack pack;
        private ILearningWay learningWay;
        private int CurrentIndex;

        public LearningProcess()
        {
            learningWays = new HashSet<ILearningWay>
            {
                new LearningWayByTest(),
                new LearningWayByTyping(),
                new LearningWayCheckYourself()
            };
        }

        public Information Execute(string id, string message)
        {
            if (status == LearningEnum.Idles)
            {
                pack = Repository.GetPack(id, message);

                if (pack == null)
                    return new Information(new List<string> { "Этой колоды нет или она пустая :("});
                
                pack.OrderCards();
                status = LearningEnum.Start;
                return new Information(new List<string> {"Выбери способ обучения"},
                    learningWays.Select(x => x.Name).ToList());
            }
            if (status == LearningEnum.Start)
            {
                foreach (var way in learningWays)
                {
                    if (string.Equals(way.Name, message, StringComparison.CurrentCultureIgnoreCase))
                    {
                        learningWay = way;
                        break;
                    }
                }

                if (learningWay == null)
                    throw new ArgumentException();
                
                learningWay.Pack = pack;
                CurrentIndex = 0;
                var question = learningWay.SendQuestion(CurrentIndex);
                var answers = learningWay.SendPossibleAnswers();
                status = LearningEnum.Execute;
            
                return new Information(new List<string> {question},answers);
            }
            else
            {
                var result = learningWay.GetAnswer(out var answer, message);
                if (result)
                    learningWay.Pack[CurrentIndex].Statistic++;
                else
                    learningWay.Pack[CurrentIndex].Statistic--;
                
                CurrentIndex = (CurrentIndex + 1) % learningWay.Pack.Cards.Count;
                
                var question = learningWay.SendQuestion(CurrentIndex);
                var answers = learningWay.SendPossibleAnswers();
                
                return new Information(new List<string> {answer, question},answers);
            }
        }

        public Information Finish(string id)
        {
            if (pack == null)
                return new Information(new List<string> {"Вызови команду /создать"});
            
            Repository.UpdateStatisticsPack(id, learningWay.Pack);
            return new Information(new List<string> {"Отличная тренировка!"});
        }
    }
    
    public enum LearningEnum
    {
        Idles,
        Start,
        Execute
    }
}