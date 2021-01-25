using System.Collections.Generic;

namespace GodnessChatBot.Domain.DialogBranches
{
    public class CreationDialogBranch : IDialogBranch
    {
        private CreationDialogBranchState status = CreationDialogBranchState.SelectingPackName;
        private string name;
        private readonly IRepository repository;

        public CreationDialogBranch(IRepository repository) => this.repository = repository;

        public ReplyMessage Execute(string id, string message)
        {
            if (status == CreationDialogBranchState.SelectingPackName)
            {
                name = message;
                var pack = new Pack(name, new Card[] { });
                repository.AddPack(id, pack);
                status = CreationDialogBranchState.Creation;
                return new ReplyMessage(new List<string> 
                { @"Отлично! Давай начнем создавать колоду)
Отправь мне карту одним сообщением в формате:
передняя сторона
задняя сторона" });
            }
            
            var data = message.Split('\n');
            if (data.Length != 2)
            {
                return new ReplyMessage(new List<string> 
                { "Некорректная карточка, пожалуйста, введи так," +
                  " чтобы лицевая сторона карточки была в первой строке, а во второй строке задняя" });
            }

            repository.AddCardInPack(id, name, new Card(data[0], data[1]));
            return new ReplyMessage(new List<string> { "Запомнил" });
        }

        public ReplyMessage Finish(string id) => new ReplyMessage(new List<string> {"Готово!"});
    }

    public enum CreationDialogBranchState
    {
        SelectingPackName,
        Creation
    }
}