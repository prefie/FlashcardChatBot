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
        private static Teacher teacher;
        private static bool isAskedPack;
        private static bool isLearningPack;
        private static bool isPackName;
        private static bool isPackCreation;

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

        private static void BotOnMessage(object sender, MessageEventArgs e)
        {
            var message = e.Message;
            
            //TODO : delete this
            Console.WriteLine($"{message.From.FirstName} {message.From.LastName} отправил сообщение боту: {message.Text}");

            foreach (var command in Commands)
            {
                if (command.Contains(message.Text))
                {
                    command.Execute(message, bot);
                    break;
                }
            }


//                 default:
//                     if (isPackName)
//                     {
//                         var pack = new Pack(message.Text, new Card[] { }, true);
//                         TeachersRepository.AddPack(message.From.Id.ToString(), pack);
//                         UpdateStatus();
//                         isPackCreation = true;
//                         await bot.SendTextMessageAsync(
//                             message.From.Id,
//                             @"Отлично! Давай начнем создавать колоду)
// Отправь мне ее одним сообщением в формате:
// передняя сторона
// задняя сторона");
//                     }
//
//                     else if (isPackCreation)
//                     {
//                         var button = GetButtons(new List<string>() {"Завершить"});
//                         var data = message.Text.Split('\n');
//                         if (data.Length != 2)
//                         {
//                             await bot.SendTextMessageAsync(message.From.Id,
//                                 "Некорректная карточка, пожалуйста, введи так," +
//                                  " чтобы лицевая сторона карточи была в первой строке, а во второй строке задняя",
//                                 replyMarkup: button);
//                             return;
//                         }
//
//                         TeachersRepository.AddCardInPack(message.From.Id.ToString(), teacher.CurrentPack.Name,
//                             new Card(data[0], data[1]));
//                         await bot.SendTextMessageAsync(message.From.Id, "Запомнил", replyMarkup:button);
//                     }
//                     else if (isLearningPack)
//                     {
//                     }
//                     break;
//            }
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

            if (callbackQuery.Data == "Завершить")
            {
                UpdateStatus();
                await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Готово!");
            }
            if (callbackQuery.Data == "Закончить обучение")
            {
                UpdateStatus();
                Repository.UpdateStatisticsPack(callbackQuery.From.Id.ToString(), teacher.CurrentPack);
                await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Отличная тренировка!");
            }

            if (isAskedPack)
            {
                var pack = Repository.GetPack(callbackQuery.From.Id.ToString(), callbackQuery.Data);
                if (pack == null)
                {
                    await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id,
                        "Этой колоды нет или она пустая, давай создадим ее!");
                    return;
                }
                teacher = new Teacher(pack);
                teacher.StartLearning(new LearningWayByTest(pack));
                
                var message = teacher.GetFaceCard();
                var options = teacher.GetPossibleAnswers().ToList();
                options.Add("Закончить обучение");
                UpdateStatus();
                isLearningPack = true;
                
                await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Начинаем!");
                await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, message,
                    replyMarkup:GetButtons(options));
            }

            else if (isLearningPack)
            {
                var answer = teacher.MakeStatisticByAnswerResult(callbackQuery.Data);
                var message = teacher.GetFaceCard();
                var options = teacher.GetPossibleAnswers().ToList();
                options.Add("Закончить обучение");
                
                await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, answer);
                await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, message,
                    replyMarkup:GetButtons(options));
            }

            await bot.EditMessageReplyMarkupAsync(e.CallbackQuery.Message.Chat.Id,
                e.CallbackQuery.Message.MessageId);
        }

        private static void UpdateStatus()
        {
            isAskedPack = false;
            isPackCreation = false;
            isPackName = false;
            isLearningPack = false;
        }
    }
}