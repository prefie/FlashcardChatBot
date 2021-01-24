using System;
using GodnessChatBot.App;

namespace GodnessChatBot
{
    internal static class Program
    {
        public static void Main()
        {
            var bot = new TelegramBot();
            bot.Start();
            Console.ReadKey();
            bot.Stop();
        }
    }
}