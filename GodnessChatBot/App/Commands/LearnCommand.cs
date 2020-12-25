using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GodnessChatBot
{
    public class LearnCommand : Command
    {
        public override string Name { get; set; } = "/учить";
        public override async void Execute(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var keyboard = GetButtons(Repository.GetPacksNames(message.From.Id.ToString()).ToList());
            
            Bot.teachers[chatId.ToString()] = new Teacher(chatId.ToString(), new LearningProcess());
            await client.SendTextMessageAsync(chatId, "Выбери колоду, которую хочешь учить", replyMarkup:keyboard);
        }
    }
}