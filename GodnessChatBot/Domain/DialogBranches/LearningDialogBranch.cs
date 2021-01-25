using System;
using System.Collections.Generic;
using System.Linq;
using GodnessChatBot.Domain.LearningWays;

namespace GodnessChatBot.Domain.DialogBranches
{
    public class LearningDialogBranch : IDialogBranch
    {
        private LearningDialogBranchState status = LearningDialogBranchState.SelectingPack;
        private readonly HashSet<LearningWay> learningWays;
        
        private Pack pack;
        private LearningWay learningWay;
        private int currentIndex;
        private readonly IRepository repository;

        public LearningDialogBranch(IRepository repository, HashSet<LearningWay> learningWays)
        {
            this.repository = repository;
            this.learningWays = learningWays;
        }

        public ReplyMessage Execute(string id, string message)
        {
            if (status == LearningDialogBranchState.SelectingPack)
            {
                pack = repository.GetPack(id, message);

                if (pack == null)
                    return new ReplyMessage(new List<string>
                        {"Этой колоды нет, она пустая или в ней ошибка"});
                
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
                
                currentIndex = 0; 
                status = LearningDialogBranchState.Learning;
            
                return learningWay.Learn(null, pack[currentIndex], pack, null);
            }

            var previousCard = pack[currentIndex];
            if (learningWay.NeedNextCard)
                currentIndex = (currentIndex + 1) % pack.Cards.Count;
            var nextCard = pack[currentIndex];

            return learningWay.Learn(previousCard, nextCard, pack, message);
        }

        public ReplyMessage Finish(string id)
        {
            if (pack == null)
                return new ReplyMessage(new List<string>
                    {"Вызови команду «Получить ссылку на таблицу» и проверь колоду :)"});
            
            repository.UpdateStatisticsPack(id, pack);
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