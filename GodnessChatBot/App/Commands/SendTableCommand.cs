using GodnessChatBot.Domain;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GodnessChatBot.App.Commands
{
    public class SendTableCommand : Command
    {
        public override string Name => "/получить ссылку на таблицу";
        public override async void Execute(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            TelegramBot.DialogBranches[chatId.ToString()] = null;
            await client.SendTextMessageAsync(chatId, repository.GetSpreadsheetUrl(chatId.ToString()));
        }

        public SendTableCommand(Repository repository) : base(repository) { }
    }
}