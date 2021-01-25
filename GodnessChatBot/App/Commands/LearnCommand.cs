using System.Collections.Generic;
using System.Linq;
using GodnessChatBot.Domain;
using GodnessChatBot.Domain.Processes;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace GodnessChatBot.App.Commands
{
    public class LearnCommand : Command
    {
        public override string Name => "Учить";
        private Dictionary<string, IDialogBranch> dialogs;
        public override async void Execute(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var keyboard = TelegramBot.GetButtons(repository.GetPacksNames(message.From.Id.ToString()).ToList());
            
            dialogs[chatId.ToString()] = new LearningDialogBranch(repository);
            await client.SendTextMessageAsync(chatId, 
                "Выбери колоду, которую хочешь учить\n\nВАЖНО: для вызова другой команды, заверши эту :)",
                replyMarkup: keyboard);
        }

        public LearnCommand(Repository repository, Dictionary<string, IDialogBranch> dialogs) : base(repository)
        {
            this.dialogs = dialogs;
        }
    }
}