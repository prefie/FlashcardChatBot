using System;
using System.Collections.Generic;
using System.Linq;
using GodnessChatBot.Domain;
using GodnessChatBot.Domain.Processes;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GodnessChatBot.App.Commands
{
    public class AdditionCommand : Command
    {
        public override string Name => "Добавить карту в колоду";
        private Dictionary<string, IDialogBranch> dialogs;
        private Func<AdditionDialogBranch> createAdditionDialogBranch;
        public override async void Execute(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var keyboard = TelegramBot.GetButtons(repository.GetPacksNames(message.From.Id.ToString()).ToList());

            dialogs[chatId.ToString()] = createAdditionDialogBranch();
            await client.SendTextMessageAsync(chatId, 
                "Выбери колоду, в которые будешь добавлять карты\n\nВАЖНО: для вызова другой команды, заверши эту :)",
                replyMarkup:keyboard);
        }

        public AdditionCommand(Repository repository, Dictionary<string, IDialogBranch> dialogs,
            Func<AdditionDialogBranch> createAdditionDialogBranch) : base(repository)
        {
            this.dialogs = dialogs;
            this.createAdditionDialogBranch = createAdditionDialogBranch;
        }
    }
}