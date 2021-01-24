using System;
using GodnessChatBot.Domain;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GodnessChatBot.App.Commands
{
    public abstract class Command
    {
        public Repository repository;
        public Command(Repository repository)
        {
            this.repository = repository;
        }
        public abstract string Name { get; }

        public abstract void Execute(Message message, TelegramBotClient client);

        public bool Equals(string command) =>
            string.Equals(command, Name, StringComparison.CurrentCultureIgnoreCase);
    }
}