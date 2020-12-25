using Telegram.Bot;
using Telegram.Bot.Types;

namespace GodnessChatBot
{
    public class HelpCommand : Command
    {
        public override string Name { get; set; } = "/help";
        public override async void Execute(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            Bot.teachers[chatId.ToString()] = new Teacher(chatId.ToString());
            await client.SendTextMessageAsync(chatId, helpMessage);
        }
        
        private static readonly string helpMessage =
            "Привет! Я – чат-бот Богиня, твой главный помощник в быстром," +
            " простом и эффективном обучении. Давай расскажу, что я умею. " +
            "Моя главная миссия - помогать людям учить нужную им информацию," +
            " и я придумал для этого отличный способ - флэш карточки. " +
            "Я умею создавать колоды карточек, для этого дай мне команду: " +
            "“Создать колоду”, выбери для неё название, и мы приступим к её заполнению." +
            " Во время этого процесса я подскажу тебе, что делать. " +
            "Существующие колоды можно учить (команда “Учить колоду”), " +
            "изменять (команда “Изменить колоду”), делиться ими с друзьями (команда “Поделиться колодой”). "; 
    }
}