using Telegram.Bot;
using Telegram.Bot.Types;

namespace GodnessChatBot
{
    public class SendTableCommand : Command
    {
        public override string Name { get; set; } = "/получить ссылку на таблицу";
        public override async void Execute(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            Bot.teachers[chatId.ToString()] = new Teacher(chatId.ToString());
            await client.SendTextMessageAsync(chatId, Repository.GetSpreadsheetUrl(chatId.ToString()));
        }
    }
}