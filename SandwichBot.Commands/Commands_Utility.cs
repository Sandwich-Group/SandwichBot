using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HoLLy.DiscordBot.Commands;
using NCalc2;

namespace HoLLy.DiscordBot.Sandwich
{
    public static partial class Commands
    {
        [Command("ping", "Returns 'Pong!'")]
        public static string Ping() => "Pong!";

        [Command("choose", "Picks a random word")]
        public static string Choose(string[] args) => args[new Random().Next(args.Length)];

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

        [Command("faq", "Returns a hardcoded FAQ item")]
        public static string Faq(string key)
        {
            key = key.ToLowerInvariant();
            return Constants.FAQ.ContainsKey(key)
                ? Constants.FAQ[key]
                : "FAQ entry not found. Try the faq-items command.";
        }

        [Command("faq-items", "Lists all items in the hardcoded FAQ")]
        public static string FaqItems() => string.Join(", ", Constants.FAQ.Keys.Select(k => $"`{k}`"));
    }
}
