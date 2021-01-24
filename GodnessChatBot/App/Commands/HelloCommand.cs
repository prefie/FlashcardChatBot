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
            
            repository.CreateSpreadsheet(message.From.Id.ToString());
            var replyKeyboard = new ReplyKeyboardMarkup(new []
                {
                    new[]
                    {
                        new KeyboardButton("/Создать"),
                        new KeyboardButton("/Учить")
                    },
                    new []
                    {
                        new KeyboardButton("/Добавить карту в колоду"),
                        new KeyboardButton("/Отправить")
                    },
                    new []
                    {
                        new KeyboardButton("/Получить ссылку на таблицу")
                    }
                }, true,
                true);
            
            TelegramBot.DialogBranches[chatId.ToString()] = null;
            await client.SendTextMessageAsync(chatId, "Давай начнем :)", replyMarkup: replyKeyboard);
        }

        public HelloCommand(Repository repository) : base(repository) { }
    }
}