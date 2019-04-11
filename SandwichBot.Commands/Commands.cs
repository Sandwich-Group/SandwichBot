using System;
using System.IO;
using System.Threading.Tasks;
using Discord.WebSocket;
using HoLLy.DiscordBot.Commands;
using HoLLy.DiscordBot.Commands.DependencyInjection;
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

        [Command("tts", "Transforms a piece of text into an audio file")]
        public static void TextToSpeech([DI] SocketMessage msg, string src)
        {
            byte[] bytes;
            string lang;
            int idx = src.IndexOf(' ');
            if (idx > 0 && idx != src.Length - 1 && GoogleTranslate.SupportsLanguage(lang = src.Substring(0, idx)))
                bytes = GoogleTranslate.TextToSpeech(src.Substring(idx + 1), lang).Result;
            else
                bytes = GoogleTranslate.TextToSpeech(src).Result;

            msg.Channel.SendFileAsync(new MemoryStream(bytes), "audio.mp3").Wait();
        }

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
