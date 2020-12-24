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
            if (!Bot.teachers.ContainsKey(chatId.ToString()))
                Bot.teachers.Add(chatId.ToString(), new Teacher(chatId.ToString(), new CreationProcess()));
            else
                Bot.teachers[chatId.ToString()] = new Teacher(chatId.ToString(), new CreationProcess());
            await client.SendTextMessageAsync(chatId, "Выбери название колоды");
        }
    }
}