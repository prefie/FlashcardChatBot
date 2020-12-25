using System;
using GodnessChatBot.App;

namespace GodnessChatBot
{
    internal static class Program
    {
        public static void Main()
        { 
            TelegramBot.Start();
            Console.ReadKey();
            TelegramBot.Stop();
        }
    }
}