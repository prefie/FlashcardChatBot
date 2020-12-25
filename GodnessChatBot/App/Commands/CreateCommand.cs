using Telegram.Bot;
using Telegram.Bot.Types;

namespace GodnessChatBot
{
    public class CreateCommand : Command
    {
        public override string Name { get; set; } = "/создать";
        public override async void Execute(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            
            Bot.teachers[chatId.ToString()] = new Teacher(chatId.ToString(), new CreationProcess());
            await client.SendTextMessageAsync(chatId, "Выбери название колоды");
        }
    }
}