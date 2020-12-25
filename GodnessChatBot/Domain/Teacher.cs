using System;
using System.Collections.Generic;
using System.Linq;

namespace GodnessChatBot
{
    public class Teacher
    {
        private string id;
        private IProcess Process { get; }

        public Teacher(string id, IProcess process=null)
        {
            this.id = id;
            Process = process;
        }

        public Information CheckStatusAndReturnAnswer(string message)
        {
            return Process == null
                ? new Information(new List<string> {"Я такого не знаю :("})
                : Process.Execute(id, message);
        }

        public Information FinishProcess() => Process.Finish(id);
    }
}