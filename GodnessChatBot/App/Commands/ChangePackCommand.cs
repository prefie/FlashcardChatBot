using Telegram.Bot;
using Telegram.Bot.Types;

namespace GodnessChatBot
{
    public class ChangePackCommand : Command
    {
        public override string Name { get; set; } = "/изменить";
        public override async void Execute(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            Bot.teachers[chatId.ToString()] = new Teacher(chatId.ToString());
            await client.SendTextMessageAsync(chatId, "Прости, я пока что это не умею( ");
        }
    }
}