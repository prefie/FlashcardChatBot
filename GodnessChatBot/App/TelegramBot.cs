using System;
using System.Collections.Generic;
using GodnessChatBot.App.Commands;
using GodnessChatBot.Domain;
using GodnessChatBot.Domain.DialogBranches;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace GodnessChatBot.App
{
    public class TelegramBot
    {
        private readonly TelegramBotClient bot;
        private readonly Command[] commands;
        private readonly Dictionary<string, IDialogBranch> dialogBranches;

        public TelegramBot(Command[] commands,
            TelegramBotClient bot, Dictionary<string, IDialogBranch> dialogBranches)
        {
            this.commands = commands;
            this.bot = bot;
            this.dialogBranches = dialogBranches;
        }

        public void Start()
        {
            bot.OnMessage += BotOnMessage;
            bot.OnCallbackQuery += BotOnCallbackQueryReceived;
            bot.StartReceiving();
        }

        public void Stop() => bot.StopReceiving();

        private async void BotOnMessage(object sender, MessageEventArgs e)
        {
            var message = e.Message;
            var userId = message.From.Id.ToString();
            try
            {
                if (!dialogBranches.ContainsKey(userId))
                    dialogBranches.Add(userId, null);

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
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                await bot.SendTextMessageAsync(message.From.Id, "Что-то пошло не так :(\nПопробуй позже");
            }
        }
        
        private async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs e)
        {
            var callbackQuery = e.CallbackQuery;
            var userId = callbackQuery.From.Id.ToString();
            try
            {
                await bot.EditMessageReplyMarkupAsync(e.CallbackQuery.Message.Chat.Id,
                    e.CallbackQuery.Message.MessageId);

                if (!dialogBranches.ContainsKey(userId))
                {
                    await bot.SendTextMessageAsync(callbackQuery.From.Id,
                        @"Извини, я тебя не понял, давай начнем сначала :( 
Вызови команду /help, и я расскажу тебе, что я умею :)");
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
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                await bot.SendTextMessageAsync(callbackQuery.From.Id, "Что-то пошло не так :(\nПопробуй позже");
            }
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