using GodnessChatBot.Domain;
using GodnessChatBot.Domain.Processes;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GodnessChatBot.App.Commands
{
    public class CreateCommand : Command
    {
        public override string Name => "/создать";
        public override async void Execute(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            
            TelegramBot.Teachers[chatId.ToString()] = new Teacher(chatId.ToString(), new CreationProcess());
            await client.SendTextMessageAsync(chatId, "Выбери название колоды");
        }
    }
}