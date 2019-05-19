using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using HoLLy.DiscordBot.Commands;
using HoLLy.DiscordBot.Commands.DependencyInjection;

namespace HoLLy.DiscordBot.Sandwich
{
    public static partial class Commands
    {
        [Command(200, "stop")]
        public static void StopBot([DI] DiscordSocketClient cl)
        {
            cl.Disconnected += ex => {
                Environment.Exit(0);
                return Task.CompletedTask;
            };
            cl.StopAsync().Wait();
        }
    }
}
