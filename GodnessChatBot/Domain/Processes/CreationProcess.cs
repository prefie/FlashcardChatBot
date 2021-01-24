using System;
using System.Collections.Generic;

namespace GodnessChatBot.Domain.Processes
{
    public class CreationProcess : IProcess
    {
        private CreationEnum status = CreationEnum.Start;
        private string name;

        public Information Execute(string id, string message)
        {
            if (status == CreationEnum.Start)
            {
                name = message;
                var pack = new Pack(name, new Card[] { });
                Repository.AddPack(id, pack);
                status = CreationEnum.Execute;
                return new Information(new List<string> 
                { @"Отлично! Давай начнем создавать колоду)
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

    public enum CreationEnum
    {
        Start,
        Execute
    }
}