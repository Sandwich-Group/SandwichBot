using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HoLLy.DiscordBot.Sandwich.Tools
{
    public static class GoogleTranslate
    {
        private const string ApiEndpoint = "https://translate.google.com/translate_a/single";
        private static readonly Dictionary<string, string> Languages = new Dictionary<string, string> {
            {"auto", "Automatic"}, {"af", "Afrikaans"}, {"sq", "Albanian"}, {"am", "Amharic"}, {"ar", "Arabic"}, {"hy", "Armenian"},
            {"az", "Azerbaijani"}, {"eu", "Basque"}, {"be", "Belarusian"}, {"bn", "Bengali"}, {"bs", "Bosnian"}, {"bg", "Bulgarian"},
            {"ca", "Catalan"}, {"ceb", "Cebuano"}, {"ny", "Chichewa"}, {"zh-cn", "Chinese Simplified"}, {"zh-tw", "Chinese Traditional"},
            {"co", "Corsican"}, {"hr", "Croatian"}, {"cs", "Czech"}, {"da", "Danish"}, {"nl", "Dutch"}, {"en", "English"}, {"eo", "Esperanto"},
            {"et", "Estonian"}, {"tl", "Filipino"}, {"fi", "Finnish"}, {"fr", "French"}, {"fy", "Frisian"}, {"gl", "Galician"}, {"ka", "Georgian"},
            {"de", "German"}, {"el", "Greek"}, {"gu", "Gujarati"}, {"ht", "Haitian Creole"}, {"ha", "Hausa"}, {"haw", "Hawaiian"}, {"iw", "Hebrew"},
            {"hi", "Hindi"}, {"hmn", "Hmong"}, {"hu", "Hungarian"}, {"is", "Icelandic"}, {"ig", "Igbo"}, {"id", "Indonesian"}, {"ga", "Irish"},
            {"it", "Italian"}, {"ja", "Japanese"}, {"jw", "Javanese"}, {"kn", "Kannada"}, {"kk", "Kazakh"}, {"km", "Khmer"}, {"ko", "Korean"},
            {"ku", "Kurdish (Kurmanji)"}, {"ky", "Kyrgyz"}, {"lo", "Lao"}, {"la", "Latin"}, {"lv", "Latvian"}, {"lt", "Lithuanian"},
            {"lb", "Luxembourgish"}, {"mk", "Macedonian"}, {"mg", "Malagasy"}, {"ms", "Malay"}, {"ml", "Malayalam"}, {"mt", "Maltese"},
            {"mi", "Maori"}, {"mr", "Marathi"}, {"mn", "Mongolian"}, {"my", "Myanmar (Burmese)"}, {"ne", "Nepali"}, {"no", "Norwegian"},
            {"ps", "Pashto"}, {"fa", "Persian"}, {"pl", "Polish"}, {"pt", "Portuguese"}, {"ma", "Punjabi"}, {"ro", "Romanian"}, {"ru", "Russian"},
            {"sm", "Samoan"}, {"gd", "Scots Gaelic"}, {"sr", "Serbian"}, {"st", "Sesotho"}, {"sn", "Shona"}, {"sd", "Sindhi"}, {"si", "Sinhala"},
            {"sk", "Slovak"}, {"sl", "Slovenian"}, {"so", "Somali"}, {"es", "Spanish"}, {"su", "Sundanese"}, {"sw", "Swahili"}, {"sv", "Swedish"},
            {"tg", "Tajik"}, {"ta", "Tamil"}, {"te", "Telugu"}, {"th", "Thai"}, {"tr", "Turkish"}, {"uk", "Ukrainian"}, {"ur", "Urdu"},
            {"uz", "Uzbek"}, {"vi", "Vietnamese"}, {"cy", "Welsh"}, {"xh", "Xhosa"}, {"yi", "Yiddish"}, {"yo", "Yoruba"}, {"zu", "Zul"},
        };

        private static string cachedTtk;

        private static int UnixEpoch => (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;

        public static async Task<string> Translate(string query, string to = "en", string from = "auto")
        {
            if (cachedTtk is null || Convert.ToInt32(cachedTtk.Split('.')[0]) != UnixEpoch / (1000*60*60))
                cachedTtk = await DownloadTTK();

            string token = GenerateToken(query, cachedTtk);

            string dt = string.Join("&", new [] { "at", "bd", "ex", "ld", "md", "qca", "rw", "rm", "ss", "t" }.Select(x => $"dt={x}"));
            string queryString = $"?client=webapp&sl={from}&th={to}&hl={to}&{dt}&source=bh&ssel=0&tsel=0&kc=1&tk={token}&q={HttpUtility.UrlEncode(query)}";

            string jsonData = await new WebClient().DownloadStringTaskAsync(ApiEndpoint + queryString);
            var data = (JArray)JsonConvert.DeserializeObject(jsonData);
            return string.Join(string.Empty, data[0].Select(x => ((JValue)x[0]).Value.ToString()));
        }

        private static async Task<string> DownloadTTK()
        {
            var resp = await new WebClient().DownloadStringTaskAsync("https://translate.google.com");
            return Regex.Match(resp, @"tkk:\'(\d+\.\d+)\'").Groups[1].Value;
        }

        internal static string GenerateToken(string inputString, string tkk)
        {
            string[] tkkSplit = tkk.Split('.');
            long tkkFirst = Convert.ToInt64(tkkSplit[0]);
            var crypted = new List<int>();
            for (var i = 0; i < inputString.Length; i++) {
                int c = inputString[i];
                if (c < 0x80)
                    crypted.Add(c);
                else {
                    if (c < 0x800) {
                        crypted.Add(c >> 6 | 0xC0);
                    } else {
                        if (0xD800 == (c & 0xFC00) && i + 1 < inputString.Length && 0xDC00 == (inputString[i + 1] & 0xFC00)) {
                            c = 0x10000 + ((c & 0x3FF) << 10) + (inputString[++i] & 0x3FF);
                            crypted.Add(c >> 18 | 0xF0);
                            crypted.Add(c >> 12 & 0x3F | 0x80);
                        } else {
                            crypted.Add(c >> 12 | 0xE0);
                            crypted.Add(c >> 6 & 0x3F | 0x80);
                        }
                    }

                    crypted.Add(c & 0x3F | 0x80);
                }
            }

            var param = tkkFirst;
            foreach (int i in crypted) {
                param = evalKey(param + i, "+-a^+6");
            }
            param = evalKey(param, "+-3^+b+-f");
            param ^= Convert.ToInt64(tkkSplit[1]);
            uint newParam = (uint)param;
            newParam %= 1000000;
            return newParam + "." + (newParam ^ tkkFirst);

            long evalKey(long input, string key) {
                Debug.WriteLine("=== " + input);
                for (int i = 0; i < key.Length - 2; i += 3) {
                    char d1 = key[i + 2];
                    int d2 = 'a' <= d1 ? d1 - 0x57 : Convert.ToInt32(d1.ToString());
                    long d = '+' == key[i + 1] ? (int)((uint)input >> d2) : input << d2;

                    input = '+' == key[i] ? (int)(input + d & 0xFFFFFFFF) : input ^ d;
                }
                Debug.WriteLine("");
                return input;
            }
        }
    }
}
