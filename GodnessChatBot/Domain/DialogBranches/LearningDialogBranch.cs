using System;
using System.Collections.Generic;
using System.Linq;
using GodnessChatBot.Domain.LearningWays;

namespace GodnessChatBot.Domain.Processes
{
    public class LearningDialogBranch : IDialogBranch
    {
        private LearningDialogBranchState status = LearningDialogBranchState.SelectingPack;
        private readonly HashSet<ILearningWay> learningWays;
        
        private Pack pack;
        private ILearningWay learningWay;
        private int currentIndex;
        
        private Repository repository;

        public LearningDialogBranch(Repository repository)
        {
            this.repository = repository;
            learningWays = new HashSet<ILearningWay>
            {
                new LearningWayByTest(),
                new LearningWayByTyping(),
                new LearningWayCheckYourself()
            };
        }

        public ReplyMessage Execute(string id, string message)
        {
            if (status == LearningDialogBranchState.SelectingPack)
            {
                pack = repository.GetPack(id, message);

                if (pack == null)
                    return new ReplyMessage(new List<string> {"Этой колоды нет, она пустая или там ошибка, проверь таблицу:("});
                
                pack.OrderCards();
                status = LearningDialogBranchState.WaitingLearningWay;
                return new ReplyMessage(new List<string> {"Выбери способ обучения"},
                    learningWays.Select(x => x.Name).ToList());
            }
            if (status == LearningDialogBranchState.WaitingLearningWay)
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
                currentIndex = 0;
                var question = learningWay.SendQuestion(currentIndex);
                var answers = learningWay.SendPossibleAnswers();
                status = LearningDialogBranchState.Learning;
            
                return new ReplyMessage(new List<string> {question},answers);
            }
            else
            {
                var result = learningWay.GetAnswer(out var answer, message);
                if (result == null)
                {
                    var quest = learningWay.SendQuestion(currentIndex);
                    var ans = learningWay.SendPossibleAnswers();
                    return new ReplyMessage(new List<string>{quest}, ans);
                }
                if (result == true)
                    learningWay.Pack[currentIndex].Statistic++;
                else
                    learningWay.Pack[currentIndex].Statistic--;
                
                currentIndex = (currentIndex + 1) % learningWay.Pack.Cards.Count;
                
                var question = learningWay.SendQuestion(currentIndex);
                var answers = learningWay.SendPossibleAnswers();
                
                return new ReplyMessage(new List<string> {answer, question},answers);
            }
        }

        public ReplyMessage Finish(string id)
        {
            if (pack == null)
                return new ReplyMessage(new List<string> {"Вызови команду /создать"});
            
            repository.UpdateStatisticsPack(id, learningWay.Pack);
            return new ReplyMessage(new List<string> {"Отличная тренировка!"});
        }
    }
    
    public enum LearningDialogBranchState
    {
        SelectingPack,
        WaitingLearningWay,
        Learning
    }
}