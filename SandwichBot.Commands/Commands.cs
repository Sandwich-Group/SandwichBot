using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord.WebSocket;
using HoLLy.DiscordBot.Commands;
using HoLLy.DiscordBot.Commands.DependencyInjection;
using HoLLy.DiscordBot.Sandwich.Tools;
using NCalc2;

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

        [Command("tr-langs", "Lists all supported languages for translation and text-to-speech")]
        public static string TranslateLanguages()
        {
            const int cols = 5;
            var items = GoogleTranslate.Languages.Select(x => (x.Key, x.Value)).ToArray();

            var lens = new int[cols];
            for (int i = 0; i < cols; i++) {
                for (int j = i; j < items.Length; j += cols) {
                    var pair = items[j];
                    var str = $"{pair.Key}: {pair.Value}";
                    lens[i] = Math.Max(lens[i], str.Length);
                }
            }

            var sb = new StringBuilder();
            for (int i = 0; i < items.Length; i++) {
                var pair = items[i];
                bool isLast = i % cols == cols  - 1;
                sb.Append($"{pair.Key}: {pair.Value}".PadRight(isLast ? 0 : lens[i % cols] + 1));

                if (isLast) sb.AppendLine();
            }

            return $"```{sb}```";
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

        [Command("roll", "Rolls a random number. Accepts DnD-style dice and number parameters.")]
        public static string Roll(string arg)
        {
            var split = arg.Split(' ');
            var rand = new Random();

            if (Regex.IsMatch(split[0], @"^\d{1,9}d\d{1,9}$")) {
                var split2 = split[0].Split('d');
                if (int.TryParse(split2[0], out int count) && int.TryParse(split2[1], out int max)) {
                    if (count > 20) return "That's a bit much, isn't it?";

                    var sb = new StringBuilder();
                    for (int i = 0; i < count; i++) {
                        sb.Append(rand.Next(max));
                        sb.Append(" ");
                    }

                    return sb.ToString();
                }
            }

            // if no ndn was given, just use parameter as 1dn
            return rand.Next(int.TryParse(split[0], out int maxValue) ? maxValue : 100).ToString();
        }

        [Command("calc", "Evaluates a mathematical expression")]
        public static string Calculate(string input)
        {
            try {
                object evaluated = new Expression(input).Evaluate();
                return evaluated is string s
                    ? $"`{s}`"
                    : evaluated.ToString();
            } catch (Exception e) {
                return e.Message;
            }
        }
    }
}
