using System;

namespace GodnessChatBot
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Bot.Start();
            Console.ReadKey();
            Bot.Stop();
        }
    }
}