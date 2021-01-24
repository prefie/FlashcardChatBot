using System;
using System.Collections.Generic;

namespace GodnessChatBot.Domain.Processes
{
    public class AdditionDialogBranch : IDialogBranch
    {
        private AdditionDialogBranchState status = AdditionDialogBranchState.SelectingPack;
        private string name;
        private Repository repository;

        public AdditionDialogBranch(Repository repository)
        {
            this.repository = repository;
        }

        public ReplyMessage Execute(string id, string message)
        {
            if (status == AdditionDialogBranchState.SelectingPack)
            {
                name = message;
                status = AdditionDialogBranchState.Addition;
                return new ReplyMessage(new List<string> 
                { @"Давай начнем добавлять карты в колоду)
Отправь мне карту одним сообщением в формате:
передняя сторона
задняя сторона" });
            }
            
            var data = message.Split('\n');
            if (data.Length != 2)
            {
                return new ReplyMessage(new List<string> 
                { "Некорректная карточка, пожалуйста, введи так," +
                  " чтобы лицевая сторона карточи была в первой строке, а во второй строке задняя" });
            }

            repository.AddCardInPack(id, name, new Card(data[0], data[1]));
            return new ReplyMessage(new List<string> { "Запомнил" });
        }

        public ReplyMessage Finish(string id)
        {
            return new ReplyMessage(new List<string> {"Готово!"});
        }
    }

    public enum AdditionDialogBranchState
    {
        SelectingPack,
        Addition
    }
}