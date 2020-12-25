using GodnessChatBot.Domain;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GodnessChatBot.App.Commands
{
    public class ChangePackCommand : Command
    {
        public override string Name => "/изменить";
        public override async void Execute(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            TelegramBot.Teachers[chatId.ToString()] = new Teacher(chatId.ToString());
            await client.SendTextMessageAsync(chatId, "Прости, я пока что это не умею( ");
        }
    }
}