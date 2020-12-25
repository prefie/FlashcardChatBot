using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net.Mime;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace GodnessChatBot
{
    public static class Bot
    {
        private static readonly TelegramBotClient bot;
        private static readonly List<Command> Commands = new List<Command>();
        public static Dictionary<string, Teacher> teachers = new Dictionary<string, Teacher>();

        static Bot()
        {
            bot = new TelegramBotClient(AppSettings.Key);
            Commands.Add(new HelloCommand());
            Commands.Add(new HelpCommand());
            Commands.Add(new CreateCommand());
            Commands.Add(new LearnCommand());
            Commands.Add(new SendPackCommand());
            Commands.Add(new SendTableCommand());
            Commands.Add(new ChangePackCommand());
        }

        public static void Start()
        {
            bot.OnMessage += BotOnMessage;
            bot.OnCallbackQuery += BotOnCallbackQueryReceived;
            bot.StartReceiving();
        }

        public static void Stop() => bot.StopReceiving();

        private static async void BotOnMessage(object sender, MessageEventArgs e)
        {
            var message = e.Message;
            var userId = message.From.Id.ToString();
            
            if (!teachers.ContainsKey(userId))
                teachers.Add(userId, new Teacher(userId));
            
            //TODO : delete this
            Console.WriteLine($"{message.From.FirstName} {message.From.LastName} отправил сообщение боту: {message.Text}");

            foreach (var command in Commands)
            {
                if (command.Contains(message.Text))
                {
                    command.Execute(message, bot);
                    return;
                }
            }

            var answer = teachers[message.From.Id.ToString()].CheckStatusAndReturnAnswer(message.Text);

            for (var i = 0; i < answer.Messages.Count - 1; i++)
                await bot.SendTextMessageAsync(message.From.Id, answer.Messages[i]);

            var last = answer.Messages[answer.Messages.Count - 1];
            
            answer.AdditionalInfo.Add("Завершить");
            var buttons = GetButtons(answer.AdditionalInfo);

            await bot.SendTextMessageAsync(message.From.Id, last, replyMarkup: buttons);

            // TODO: teachers[userId].checkStatusAndReturnAnswer() проверяем в каком процессе находимся
            // если учимся или создаем, возвращает нужный ответ, иначе "Я не понял тебя"
        }

        private static InlineKeyboardMarkup GetButtons(List<string> headers)
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
        
        private static async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs e)
        {
            var callbackQuery = e.CallbackQuery;
            var userId = callbackQuery.From.Id.ToString();
            
            await bot.EditMessageReplyMarkupAsync(e.CallbackQuery.Message.Chat.Id,
                e.CallbackQuery.Message.MessageId);

            if (!teachers.ContainsKey(userId))
            {
                await bot.SendTextMessageAsync(callbackQuery.From.Id, "Никакой процесс не запущен, начни сначала :(");
                return;
            }

            if (callbackQuery.Data == "Завершить" || callbackQuery.Data == "Закончить обучение")
            {
                var messages = teachers[userId].FinishProcess().Messages;
                foreach (var answerMessage in messages)
                    await bot.SendTextMessageAsync(callbackQuery.From.Id, answerMessage);
                return;
            }
            
            var answer = teachers[callbackQuery.From.Id.ToString()].CheckStatusAndReturnAnswer(callbackQuery.Data);

            for (var i = 0; i < answer.Messages.Count - 1; i++)
                await bot.SendTextMessageAsync(callbackQuery.From.Id, answer.Messages[i]);

            var last = answer.Messages[answer.Messages.Count - 1];
            
            answer.AdditionalInfo.Add("Завершить");
            var buttons = GetButtons(answer.AdditionalInfo);

            await bot.SendTextMessageAsync(callbackQuery.From.Id, last, replyMarkup: buttons);
            
            //
            // if (callbackQuery.Data == "Завершить") //TODO command
            // {
            //     UpdateStatus();
            //     await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Готово!");
            // }
            // if (callbackQuery.Data == "Закончить обучение")
            // {
            //     UpdateStatus();
            //     Repository.UpdateStatisticsPack(userId, teachers[userId].CurrentPack);
            //     await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Отличная тренировка!");
            // }
            //
            // if (isAskedPack)
            // {
            //     var pack = Repository.GetPack(callbackQuery.From.Id.ToString(), callbackQuery.Data);
            //     if (pack == null)
            //     {
            //         await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id,
            //             "Этой колоды нет или она пустая, давай создадим ее!");
            //         return;
            //     }
            //     teachers.Add(userId, new Teacher(userId));
            //     teachers[userId].StartLearning(new LearningWayByTest(pack));
            //
            //     
            //     var message = teachers[userId].GetFaceCard();
            //     var options = teachers[userId].GetPossibleAnswers().ToList();
            //     options.Add("Закончить обучение");
            //     UpdateStatus();
            //     isLearningPack = true;
            //     
            //     await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Начинаем!");
            //     await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, message,
            //         replyMarkup:GetButtons(options));
            // }
            //
            // else if (isLearningPack)
            // {
            //     var answer = teachers[userId].MakeStatisticByAnswerResult(callbackQuery.Data);
            //     var message = teachers[userId].GetFaceCard();
            //     var options = teachers[userId].GetPossibleAnswers().ToList();
            //     options.Add("Закончить обучение");
            //     
            //     await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, answer);
            //     await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, message,
            //         replyMarkup:GetButtons(options));
            // }
            //
        }
    }
}