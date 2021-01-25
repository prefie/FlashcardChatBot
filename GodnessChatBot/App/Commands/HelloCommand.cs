using GodnessChatBot.Domain;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace GodnessChatBot.App.Commands
{
    public class HelloCommand : Command
    {
        public override string Name => "/start";
        
        public override async void Execute(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            
            repository.CreateSpreadsheetForUser(message.From.Id.ToString());
            var replyKeyboard = new ReplyKeyboardMarkup(new []
                {
                    new[]
                    {
                        new KeyboardButton("Создать"),
                        new KeyboardButton("Учить")
                    },
                    new []
                    {
                        new KeyboardButton("Добавить карту в колоду")
                    },
                    new []
                    {
                        new KeyboardButton("Получить ссылку на таблицу")
                    }
                }, true,
                true);
            
            await client.SendTextMessageAsync(chatId, "Давай начнем :)", replyMarkup: replyKeyboard);
        }

        public HelloCommand(IRepository repository) : base(repository) { }
    }
}