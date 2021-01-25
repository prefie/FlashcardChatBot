using System;
using System.Collections.Generic;
using System.Linq;
using GodnessChatBot.Domain;
using GodnessChatBot.Domain.DialogBranches;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GodnessChatBot.App.Commands
{
    public class LearnCommand : Command
    {
        protected override string Name => "Учить";
        private readonly Dictionary<string, IDialogBranch> dialogs;
        private readonly Func<LearningDialogBranch> createLearningDialogBranch;
        
        public override async void Execute(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var keyboard = TelegramBot.GetButtons(Repository.GetPacksNames(message.From.Id.ToString()).ToList());

            dialogs[chatId.ToString()] = createLearningDialogBranch();
            await client.SendTextMessageAsync(chatId, 
                "Выбери колоду, которую хочешь учить\n\nВАЖНО: для вызова другой команды, заверши эту :)",
                replyMarkup: keyboard);
        }

        public LearnCommand(IRepository repository, Dictionary<string,
            IDialogBranch> dialogs, Func<LearningDialogBranch> createLearningDialogBranch) : base(repository)
        {
            this.dialogs = dialogs;
            this.createLearningDialogBranch = createLearningDialogBranch;
        }
    }
}