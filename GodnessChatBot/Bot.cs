using System;
using System.Collections.Generic;
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
        private static readonly TeachersRepository Repository = new TeachersRepository();
        private static Teacher teacher;
        private static Pack currentPack;
        private static bool isAskedPack;
        private static bool isLearningPack;
        private static bool isPackName;
        private static bool isPackCreation;
        private static readonly string helpMessage =
            "Привет! Я – чат-бот Богиня, твой главный помощник в быстром," +
            " простом и эффективном обучении. Давай расскажу, что я умею. " +
            "Моя главная миссия - помогать людям учить нужную им информацию," +
            " и я придумал для этого отличный способ - флэш карточки. " +
            "Я умею создавать колоды карточек, для этого дай мне команду: " +
            "“Создать колоду”, выбери для неё название, и мы приступим к её заполнению." +
            " Во время этого процесса я подскажу тебе, что делать. " +
            "Существующие колоды можно учить (команда “Учить колоду”), " +
            "изменять (команда “Изменить колоду”), делиться ими с друзьями (команда “Поделиться колодой”). " +
            "Проверить себя ты можешь с помощью теста" +
            "Если устанешь и захочешь прекратить изучение какой-то колоды, дай мне команду “Закончить обучение”"; 

        static Bot()
        {
            bot = new TelegramBotClient("1171697160:AAF75ILo0dQcaXyz84LScwO2KP2i58n4uSo");
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
            
            //TODO : delete this
            Console.WriteLine($"{message.From.FirstName} {message.From.LastName} отправил сообщение боту: {message.Text}");

            switch (message.Text.ToLower())
            {
                case "/start":
                    Repository.CreateSpreadsheet(message.From.Id.ToString());
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
                        },
                        new []
                        {
                            new KeyboardButton("Получить ссылку на таблицу")
                        }
                    }, true,
                        true);
                    await bot.SendTextMessageAsync(message.From.Id, "Давай начнем:)", replyMarkup: replyKeyboard);
                    break;
                case "/help":
                    UpdateStatus();
                    await bot.SendTextMessageAsync(message.From.Id, helpMessage);
                    break;
                case "учить":
                    UpdateStatus();
                    isAskedPack = true;
                    var keyboard = GetButtons(Repository.GetPacksNames(message.From.Id.ToString()).ToList());
                    await bot.SendTextMessageAsync(message.From.Id, "Выбери колоду, которую хочешь учить", replyMarkup:keyboard);
                    break;
                case "создать":
                    UpdateStatus();
                    isPackName = true;
                    await bot.SendTextMessageAsync(message.From.Id, "Выбери название колоды");
                    break;
                case "изменить":
                    UpdateStatus();
                    await bot.SendTextMessageAsync(message.From.Id, "Прости, я пока что это не умею( ");
                    break;
                case "отправить":
                    UpdateStatus();
                    await bot.SendTextMessageAsync(message.From.Id, "Прости, я пока что это не умею( ");
                    break;
                case "получить ссылку на таблицу":
                    UpdateStatus();
                    await bot.SendTextMessageAsync(message.From.Id, Repository.GetSpreadsheetUrl(message.From.Id.ToString()));
                    break;
                default:
                    if (isPackName)
                    {
                        currentPack = new Pack(message.Text, new Card[] { }, true);
                        Repository.AddPack(message.From.Id.ToString(), currentPack);
                        UpdateStatus();
                        isPackCreation = true;
                        await bot.SendTextMessageAsync(
                            message.From.Id,
                            @"Отлично! Давай начнем создавать колоду)
Отправь мне ее одним сообщением в формате:
передняя сторона
задняя сторона");
                    }

                    else if (isPackCreation)
                    {
                        var button = GetButtons(new List<string>() {"Завершить"});
                        var data = message.Text.Split('\n');
                        if (data.Length != 2)
                        {
                            await bot.SendTextMessageAsync(message.From.Id,
                                "Некорректная карточка, пожалуйста, введи так," +
                                 " чтобы лицевая сторона карточи была в первой строке, а во второй строке задняя",
                                replyMarkup: button);
                            return;
                        }

                        Repository.AddCardInPack(message.From.Id.ToString(), currentPack.Name,
                            new Card(data[0], data[1]));
                        await bot.SendTextMessageAsync(message.From.Id, "Запомнил", replyMarkup:button);
                    }
                    else if (isLearningPack)
                    {
                    }
                    break;
            }
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
                Repository.UpdateStatisticsPack(callbackQuery.From.Id.ToString(), currentPack);
                currentPack = null;
                await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Отличная тренировка!");
            }

            if (isAskedPack)
            {
                currentPack = Repository.GetPack(callbackQuery.From.Id.ToString(), callbackQuery.Data);
                if (currentPack == null)
                {
                    await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id,
                        "Этой колоды нет или она пустая, давай создадим ее!");
                    return;
                }
                teacher = new Teacher(currentPack);
                
                teacher.StartLearning(new LearningWayByTest(currentPack));
                
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