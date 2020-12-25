using System;
using System.Collections.Generic;

namespace GodnessChatBot.Domain.Processes
{
    public class AdditionProcess : IProcess
    {
        private AdditionEnum status = AdditionEnum.Start;
        private string name;

        public Information Execute(string id, string message)
        {
            if (status == AdditionEnum.Start)
            {
                if (status != AdditionEnum.Start)
                    throw new InvalidOperationException();
            
                name = message;
                status = AdditionEnum.Execute;
                return new Information(new List<string> 
                { @"Давай начнем добавлять карты в колоду)
Отправь мне карту одним сообщением в формате:
передняя сторона
задняя сторона" });
            }
            
            var data = message.Split('\n');
            if (data.Length != 2)
            {
                return new Information(new List<string> 
                { "Некорректная карточка, пожалуйста, введи так," +
                  " чтобы лицевая сторона карточи была в первой строке, а во второй строке задняя" });
            }

            Repository.AddCardInPack(id, name, new Card(data[0], data[1]));
            return new Information(new List<string> { "Запомнил" });
        }

        public Information Finish(string id)
        {
            return new Information(new List<string> {"Готово!"});
        }
    }

    public enum AdditionEnum
    {
        Start,
        Execute
    }
}