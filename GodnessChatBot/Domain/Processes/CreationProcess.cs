using System;
using System.Collections.Generic;

namespace GodnessChatBot
{
    public class CreationProcess : IProcess
    {
        private CreationEnum status = CreationEnum.Start;
        private string name;
        
        public Telegramma Start(string id, object obj)
        {
            if (status != CreationEnum.Start)
                throw new InvalidOperationException();
            
            name = obj.ToString();
            var pack = new Pack(name, new Card[] { }, true);
            Repository.AddPack(id, pack);
            status = CreationEnum.Execute;
            return new Telegramma(new List<string> 
            { @"Отлично! Давай начнем создавать колоду)
Отправь мне ее одним сообщением в формате:
передняя сторона
задняя сторона" });
        }

        public Telegramma Execute(string id, string message)
        {
            if (status != CreationEnum.Execute)
                throw new InvalidOperationException();
            
            var data = message.Split('\n');
            if (data.Length != 2)
            {
                return new Telegramma(new List<string> 
                { "Некорректная карточка, пожалуйста, введи так," +
                  " чтобы лицевая сторона карточи была в первой строке, а во второй строке задняя" });
            }

            Repository.AddCardInPack(id, name, new Card(data[0], data[1]));
            return new Telegramma(new List<string> { "Запомнил" });
        }

        public Telegramma Finish(string id)
        {
            return new Telegramma(new List<string> {"Готово!"});
        }
    }

    public enum CreationEnum
    {
        Start,
        Execute
    }
}