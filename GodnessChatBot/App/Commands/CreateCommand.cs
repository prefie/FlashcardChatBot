using GodnessChatBot.Domain;
using GodnessChatBot.Domain.Processes;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GodnessChatBot.App.Commands
{
    public class CreateCommand : Command
    {
        public override string Name => "Создать";
        public override async void Execute(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            
            TelegramBot.DialogBranches[chatId.ToString()] = new CreationDialogBranch(repository);
            await client.SendTextMessageAsync(chatId, "Выбери название колоды\n\nВАЖНО: для вызова другой команды, заверши эту :)");
        }

        public CreateCommand(Repository repository) : base(repository) { }
    }
}