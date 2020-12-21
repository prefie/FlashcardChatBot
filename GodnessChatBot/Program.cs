using System;

namespace GodnessChatBot
{
    internal class Program
    {
        public static void Main()
        { 
            Bot.Start();
            Console.ReadKey();
            Bot.Stop();
        }
    }
}