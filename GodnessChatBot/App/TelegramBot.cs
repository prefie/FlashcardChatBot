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
    public class TelegramBot
    {
        private readonly TelegramBotClient bot;
        private readonly List<Command> commands = new List<Command>();
        private readonly Dictionary<string, IDialogBranch> dialogBranches = new Dictionary<string, IDialogBranch>();
        private readonly Repository repository = new Repository();

        public TelegramBot()
        {
            bot = new TelegramBotClient(AppSettings.Key);
            commands.Add(new HelloCommand(repository));
            commands.Add(new HelpCommand(repository));
            commands.Add(new SendPackCommand(repository));
            commands.Add(new SendTableCommand(repository));
            commands.Add(new CreateCommand(repository, dialogBranches));
            commands.Add(new LearnCommand(repository, dialogBranches));
            commands.Add(new AdditionCommand(repository, dialogBranches));
        }

        public void Start()
        {
            bot.OnMessage += BotOnMessage;
            bot.OnCallbackQuery += BotOnCallbackQueryReceived;
            bot.StartReceiving();
        }

        public void Stop() => bot.StopReceiving();

        private void BotOnMessage(object sender, MessageEventArgs e)
        {
            var message = e.Message;
            var userId = message.From.Id.ToString();
            
            if (!dialogBranches.ContainsKey(userId))
                dialogBranches.Add(userId, null);
            
            Console.WriteLine($"{message.From.FirstName} {message.From.LastName} отправил сообщение боту: {message.Text}");

            if (dialogBranches[userId] == null)
                foreach (var command in commands)
                {
                    if (command.Equals(message.Text))
                    {
                        command.Execute(message, bot);
                        return;
                    }
                }
            
            SendMessages(message.From.Id.ToString(), message.Text);
        }
        
        private async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs e)
        {
            var callbackQuery = e.CallbackQuery;
            var userId = callbackQuery.From.Id.ToString();
            
            await bot.EditMessageReplyMarkupAsync(e.CallbackQuery.Message.Chat.Id,
                e.CallbackQuery.Message.MessageId);

            if (!dialogBranches.ContainsKey(userId))
            {
                await bot.SendTextMessageAsync(callbackQuery.From.Id,
                    @"Извини, я тебя не понял, давай начнем сначала :( 
Вызови команду /help и я расскажу тебе, что я умею :)");
                return;
            }

            if (callbackQuery.Data == "Завершить" || callbackQuery.Data == "Закончить обучение")
            {
                if (dialogBranches[userId] == null) return;
                var messages = dialogBranches[userId].Finish(userId).Messages;

                dialogBranches[userId] = null;
                await bot.SendTextMessageAsync(callbackQuery.From.Id, messages);
                return;
            }
            
            SendMessages(callbackQuery.From.Id.ToString(), callbackQuery.Data);
        }

        private async void SendMessages(string id, string message)
        {
            var answer = dialogBranches[id] == null
                ? new ReplyMessage(new List<string> {"Я такого не знаю :("})
                : dialogBranches[id].Execute(id, message);
            
            if (dialogBranches[id] != null)
                answer.ReplyOptions.Add("Завершить");
            
            var buttons = GetButtons(answer.ReplyOptions);

            await bot.SendTextMessageAsync(id, answer.Messages, replyMarkup: buttons);
        }
        
        public static IReplyMarkup GetButtons(List<string> headers)
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