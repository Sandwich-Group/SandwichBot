using System.Linq;

namespace HoLLy.DiscordBot.Sandwich.Tools
{
    public static class StringHelper
    {
        private static readonly char[] ToEscape = { '\\', '`', '_', '*', '~' };

        public static string Escape(this string s) => ToEscape.Select(x => x.ToString())
                                                              .Aggregate(s, (current, c) => current.Replace(c, '\\' + c));

        public static string Unescape(this string s) => ToEscape.Select(x => x.ToString())
                                                                .Aggregate(s, (current, c) => current.Replace('\\' + c, c));
    }
}
