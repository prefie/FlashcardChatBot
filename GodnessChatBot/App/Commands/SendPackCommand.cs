using GodnessChatBot.Domain;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GodnessChatBot.App.Commands
{
    public class SendPackCommand : Command
    {
        protected override string Name => "Отправить";
        
        public override async void Execute(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            await client.SendTextMessageAsync(chatId, "Прости, я пока что это не умею( ");
        }

        public SendPackCommand(IRepository repository) : base(repository) { }
    }
}