using System;
using System.Collections.Generic;

namespace GodnessChatBot.Domain.LearningWays
{
    public class LearningWayCheckYourself : LearningWay
    {
        public override string Name => "Самопроверка";
        private LearningCheckYourselfState state = LearningCheckYourselfState.WaitingFirstCard;
        public LearningWayCheckYourself() => NeedNextCard = false;
        
        
        public override ReplyMessage Learn(Card previousCard, Card card, Pack pack, string message)
        {
            if (state == LearningCheckYourselfState.WaitingFirstCard)
            {
                state = LearningCheckYourselfState.WaitingAnotherCard;
                return new ReplyMessage(new List<string>{card.Face}, 
                    new List<string>{"Показать ответ"});
            }
            
            NeedNextCard = string.Equals(message, "Показать ответ", StringComparison.InvariantCultureIgnoreCase);
            var question = NeedNextCard ? previousCard.Back : card.Face;

            if (NeedNextCard)
            {
                return new ReplyMessage(new List<string>{question}, 
                    new List<string> {"Помню", "Не помню"});
            }

            if (!string.Equals(message, "Помню", StringComparison.CurrentCultureIgnoreCase)
                && !string.Equals(message, "Не помню", StringComparison.CurrentCultureIgnoreCase))
            {
                return new ReplyMessage(new List<string> 
                    { "Недопустимый вариант ответа, нажми на одну из кнопок сообщения выше :)" });
            }
            
            CalculateStatistic(previousCard,
                string.Equals(message, "Помню", StringComparison.CurrentCultureIgnoreCase));
            return new ReplyMessage(new List<string>{"Хорошо, идём дальше", question}, 
                    new List<string>{"Показать ответ"});
        }
    }

    public enum LearningCheckYourselfState
    {
        WaitingFirstCard,
        WaitingAnotherCard
    }
}