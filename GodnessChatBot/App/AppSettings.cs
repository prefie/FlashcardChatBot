using System;

namespace GodnessChatBot.App
{
    public static class AppSettings
    {
        public static string Url => "";
        public static string Name => "GodnessChatBot";
        public static string Key => Environment.GetEnvironmentVariable("BotKey");
    }
}