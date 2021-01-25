using System;
using System.Collections.Generic;
using GodnessChatBot.Domain;
using GodnessChatBot.Domain.DialogBranches;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GodnessChatBot.App.Commands
{
    public class CreateCommand : Command
    {
        protected override string Name => "Создать";
        private readonly Dictionary<string, IDialogBranch> dialogs;
        private readonly Func<CreationDialogBranch> createCreationDialogBranch;
        
        public override async void Execute(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;

            dialogs[chatId.ToString()] = createCreationDialogBranch();
            await client.SendTextMessageAsync(
                chatId, "Выбери название колоды\n\nВАЖНО: для вызова другой команды, заверши эту :)");
        }

        public CreateCommand(IRepository repository, Dictionary<string, IDialogBranch> dialogs,
            Func<CreationDialogBranch> createCreationDialogBranch) : base(repository)
        {
            this.dialogs = dialogs;
            this.createCreationDialogBranch = createCreationDialogBranch;
        }
    }
}