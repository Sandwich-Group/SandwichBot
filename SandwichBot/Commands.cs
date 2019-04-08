using HoLLy.DiscordBot.Commands;
using HoLLy.DiscordBot.Sandwich.Tools;

// ReSharper disable UnusedMember.Global

namespace HoLLy.DiscordBot.Sandwich
{
    public static class Commands
    {
        [Command("ping", "Returns 'Pong!'")]
        public static string Ping() => "Pong!";

        [Command("tr_en", "Translates a string to english")]
        public static string TranslateEng(string src) => GoogleTranslate.Translate(src).Result;
    }
}
