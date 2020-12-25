using System;
using System.Collections.Generic;
using GodnessChatBot.App.Commands;
using GodnessChatBot.Domain;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace GodnessChatBot.App
{
    public static class TelegramBot
    {
        private static readonly TelegramBotClient Bot;
        private static readonly List<Command> Commands = new List<Command>();
        public static readonly Dictionary<string, Teacher> Teachers = new Dictionary<string, Teacher>();

        static TelegramBot()
        {
            Bot = new TelegramBotClient(AppSettings.Key);
            Commands.Add(new HelloCommand());
            Commands.Add(new HelpCommand());
            Commands.Add(new CreateCommand());
            Commands.Add(new LearnCommand());
            Commands.Add(new SendPackCommand());
            Commands.Add(new SendTableCommand());
            Commands.Add(new AdditionCommand());
        }

        public static void Start()
        {
            Bot.OnMessage += BotOnMessage;
            Bot.OnCallbackQuery += BotOnCallbackQueryReceived;
            Bot.StartReceiving();
        }

        public static void Stop() => Bot.StopReceiving();

        private static void BotOnMessage(object sender, MessageEventArgs e)
        {
            var message = e.Message;
            var userId = message.From.Id.ToString();
            
            if (!Teachers.ContainsKey(userId))
                Teachers.Add(userId, new Teacher(userId));
            
            //TODO : delete this
            Console.WriteLine($"{message.From.FirstName} {message.From.LastName} отправил сообщение боту: {message.Text}");

            foreach (var command in Commands)
            {
                if (command.Contains(message.Text))
                {
                    command.Execute(message, Bot);
                    return;
                }
            }
            
            SendMessages(message.From.Id.ToString(), message.Text);
        }
        
        private static async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs e)
        {
            var callbackQuery = e.CallbackQuery;
            var userId = callbackQuery.From.Id.ToString();
            
            await Bot.EditMessageReplyMarkupAsync(e.CallbackQuery.Message.Chat.Id,
                e.CallbackQuery.Message.MessageId);

            if (!Teachers.ContainsKey(userId))
            {
                await Bot.SendTextMessageAsync(callbackQuery.From.Id, "Никакой процесс не запущен, начни сначала :(");
                return;
            }

            if (callbackQuery.Data == "Завершить" || callbackQuery.Data == "Закончить обучение")
            {
                var messages = Teachers[userId].FinishProcess().Messages;
                foreach (var answerMessage in messages)
                    await Bot.SendTextMessageAsync(callbackQuery.From.Id, answerMessage);
                return;
            }
            
            SendMessages(callbackQuery.From.Id.ToString(), callbackQuery.Data);
        }

        private static async void SendMessages(string id, string message)
        {
            var answer = Teachers[id].CheckStatusAndReturnAnswer(message);

            for (var i = 0; i < answer.Messages.Count - 1; i++)
                await Bot.SendTextMessageAsync(id, answer.Messages[i]);

            var last = answer.Messages[answer.Messages.Count - 1];
            
            answer.AdditionalInfo.Add("Завершить");
            var buttons = GetButtons(answer.AdditionalInfo);

            await Bot.SendTextMessageAsync(id, last, replyMarkup: buttons);
        }
        
        public static InlineKeyboardMarkup GetButtons(List<string> headers)
        {
            var buttons = new InlineKeyboardButton[headers.Count][];
            
            for (var i = 0; i < headers.Count; i++)
            {
                buttons[i] = new []
                {
                    InlineKeyboardButton.WithCallbackData(headers[i]), 
                };
            }
            
            return new InlineKeyboardMarkup(buttons);
        }
    }
}