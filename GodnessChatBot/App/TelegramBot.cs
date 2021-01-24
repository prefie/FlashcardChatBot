using System;
using System.Collections.Generic;
using GodnessChatBot.App.Commands;
using GodnessChatBot.Domain;
using GodnessChatBot.Domain.Processes;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace GodnessChatBot.App
{
    public static class TelegramBot
    {
        private static readonly TelegramBotClient Bot;
        private static readonly List<Command> Commands = new List<Command>();
        public static readonly Dictionary<string, IDialogBranch> DialogBranches = new Dictionary<string, IDialogBranch>();
        public static readonly Repository Repository = new Repository();

        static TelegramBot()
        {
            Bot = new TelegramBotClient(AppSettings.Key);
            Commands.Add(new HelloCommand(Repository));
            Commands.Add(new HelpCommand(Repository));
            Commands.Add(new CreateCommand(Repository));
            Commands.Add(new LearnCommand(Repository));
            Commands.Add(new SendPackCommand(Repository));
            Commands.Add(new SendTableCommand(Repository));
            Commands.Add(new AdditionCommand(Repository));
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
            
            if (!DialogBranches.ContainsKey(userId))
                DialogBranches.Add(userId, null);
            
            //TODO : delete this
            Console.WriteLine($"{message.From.FirstName} {message.From.LastName} отправил сообщение боту: {message.Text}");

            foreach (var command in Commands)
            {
                if (command.Equals(message.Text))
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

            if (!DialogBranches.ContainsKey(userId))
            {
                await Bot.SendTextMessageAsync(callbackQuery.From.Id,
                    @"Извини, я тебя не понял, давай начнем сначала :( 
Вызови команду /help и я расскажу тебе, что я умею :)");
                return;
            }

            if (callbackQuery.Data == "Завершить" || callbackQuery.Data == "Закончить обучение")
            {
                var messages = DialogBranches[userId].Finish(userId).Messages;
                
                await Bot.SendTextMessageAsync(callbackQuery.From.Id, messages);
                return;
            }
            
            SendMessages(callbackQuery.From.Id.ToString(), callbackQuery.Data);
        }

        private static async void SendMessages(string id, string message)
        {
            var answer = DialogBranches[id] == null
                ? new ReplyMessage(new List<string> {"Я такого не знаю :("})
                : DialogBranches[id].Execute(id, message);
            
            answer.ReplyOptions.Add("Завершить");
            var buttons = GetButtons(answer.ReplyOptions);

            await Bot.SendTextMessageAsync(id, answer.Messages, replyMarkup: buttons);
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