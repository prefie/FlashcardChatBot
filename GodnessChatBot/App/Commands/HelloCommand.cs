using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace GodnessChatBot
{
    public class HelloCommand : Command
    {
        public override string Name { get; set; } = "/start";
        
        public override async void Execute(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            
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
            await client.SendTextMessageAsync(chatId, "Давай начнем:)", replyMarkup: replyKeyboard);
        }
    }
}