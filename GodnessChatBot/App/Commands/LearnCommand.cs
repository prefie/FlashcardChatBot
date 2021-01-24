using System.Linq;
using GodnessChatBot.Domain;
using GodnessChatBot.Domain.Processes;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GodnessChatBot.App.Commands
{
    public class LearnCommand : Command
    {
        public override string Name => "/учить";
        public override async void Execute(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var keyboard = TelegramBot.GetButtons(repository.GetPacksNames(message.From.Id.ToString()).ToList());
            
            TelegramBot.DialogBranches[chatId.ToString()] = new LearningDialogBranch(repository);
            await client.SendTextMessageAsync(chatId, "Выбери колоду, которую хочешь учить", replyMarkup:keyboard);
        }

        public LearnCommand(Repository repository) : base(repository) { }
    }
}