using System;
using System.Collections.Generic;

namespace GodnessChatBot.Domain.LearningWays
{
    public class LearningWayCheckYourself : LearningWay
    {
        public override string Name => "Самопроверка";
        private LearningCheckYourselfState state = LearningCheckYourselfState.WaitingFirstCard;
        public LearningWayCheckYourself() => NeedNextCard = true;
        
        
        public override ReplyMessage Learn(Card card, Pack pack, string message)
        {
            NeedNextCard = !string.Equals(message, "Показать ответ", StringComparison.InvariantCultureIgnoreCase);
            var question = NeedNextCard ? card.Face : card.Back;

            if (!NeedNextCard)
            {
                return new ReplyMessage(new List<string>{question}, 
                    new List<string> {"Помню", "Не помню"});
            }

            if (state == LearningCheckYourselfState.WaitingFirstCard)
            {
                state = LearningCheckYourselfState.WaitingAnotherCard;
                return new ReplyMessage(new List<string>{question}, 
                    new List<string>{"Показать ответ", "Помню", "Не помню"});
            }
            
            return new ReplyMessage(new List<string>{"Хорошо, идём дальше", question}, 
                    new List<string>{"Показать ответ", "Помню", "Не помню"});
        }
    }

    public enum LearningCheckYourselfState
    {
        WaitingFirstCard,
        WaitingAnotherCard
    }
}