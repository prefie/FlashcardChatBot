using System;
using System.Collections.Generic;
using System.Linq;

namespace GodnessChatBot
{
    public class Teacher
    {
        private string id;
        public IProcess Process { get; }  

        public Teacher(string id, IProcess process)
        {
            this.id = id;
            Process = process;
        }

        public Telegramma CheckStatusAndReturnAnswer()
        {
            return new Telegramma(null);
        }
    }
}