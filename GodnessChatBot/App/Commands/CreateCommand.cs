using GodnessChatBot.Domain;
using GodnessChatBot.Domain.Processes;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GodnessChatBot.App.Commands
{
    public class CreateCommand : Command
    {
        public override string Name => "/создать";
        public override async void Execute(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            
            TelegramBot.DialogBranches[chatId.ToString()] = new CreationDialogBranch(repository);
            await client.SendTextMessageAsync(chatId, "Выбери название колоды");
        }

        public CreateCommand(Repository repository) : base(repository) { }
    }
}