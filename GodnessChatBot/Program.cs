using System;
using System.Collections.Generic;
using GodnessChatBot.App;
using GodnessChatBot.App.Commands;
using GodnessChatBot.Domain;
using GodnessChatBot.Domain.LearningWays;
using GodnessChatBot.Domain.Processes;
using Ninject;
using Ninject.Extensions.Conventions;
using Telegram.Bot;

namespace GodnessChatBot
{
    internal static class Program
    {
        public static void Main()
        {
            var bot = CreateTelegramBot();
            Console.ReadKey();
            bot.Stop();
        }

        private static TelegramBot CreateTelegramBot()
        {
            var container = new StandardKernel();
            
            container.Bind(x => x.FromThisAssembly()
                .SelectAllClasses().InheritedFrom<Command>().BindAllBaseClasses());
            container.Bind(x => x.FromThisAssembly()
                .SelectAllClasses().InheritedFrom<LearningWay>().BindAllBaseClasses());
            container.Bind<TelegramBotClient>().ToConstant(new TelegramBotClient(AppSettings.Key));
            container.Bind<Repository>().ToSelf().InSingletonScope();
            container.Bind<Dictionary<string, IDialogBranch>>().ToSelf().InSingletonScope();
            container.Bind<TelegramBot>().ToSelf()
                .OnActivation(b => b.Start());

            return container.Get<TelegramBot>();
        }
    }
}