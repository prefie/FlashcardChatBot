using System;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace GodnessChatBot
{
    public static class Bot
    {
        private static readonly TelegramBotClient bot;

        static Bot()
        {
            bot = new TelegramBotClient("1171697160:AAF75ILo0dQcaXyz84LScwO2KP2i58n4uSo");
        }

        public static void Start()
        {
            bot.OnMessage += BotOnMessage;
            bot.StartReceiving();
        }

        public static void Stop() => bot.StopReceiving();

        private static async void BotOnMessage(object sender, MessageEventArgs e)
        {
            var message = e.Message;
            
            var bot = (TelegramBotClient) sender;
            Console.WriteLine($"{message.From.FirstName} {message.From.LastName} отправил сообщение боту: {message.Text}");

            switch (message.Text)
            {
                case "/menu":
                    var replyKeyboard = new ReplyKeyboardMarkup(new []
                    {
                        new[]
                        {
                            new KeyboardButton("Создать"),
                            new KeyboardButton("Учить")
                        },
                        new []
                        {
                            new KeyboardButton("Изменить"),
                            new KeyboardButton("Отправить")
                        }
                    }, true);
                    await bot.SendTextMessageAsync(message.From.Id, "Выбирай", replyMarkup: replyKeyboard);
                    break;
            }
        }
    }
}