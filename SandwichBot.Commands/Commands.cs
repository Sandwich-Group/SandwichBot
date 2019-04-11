using System;
using HoLLy.DiscordBot.Commands;
using HoLLy.DiscordBot.Sandwich.Tools;

// ReSharper disable UnusedMember.Global

namespace HoLLy.DiscordBot.Sandwich
{
    public static class Commands
    {
        [Command("ping", "Returns 'Pong!'")]
        public static string Ping() => "Pong!";

        [Command("tr", "Translates a string")]
        public static string Translate(string src)
        {
            int idx = src.IndexOf(' ');
            if (idx > 0 && idx != src.Length - 1) {
                string dst = src.Substring(0, idx);
                if (GoogleTranslate.SupportsLanguage(dst))
                    return GoogleTranslate.Translate(src.Substring(idx + 1), dst).Result;
            }

            return GoogleTranslate.Translate(src).Result;
        }
    }
}
