using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HoLLy.DiscordBot.Sandwich.Tools
{
    public static class UrbanDictionary
    {
        private const string ApiBase = "http://api.urbandictionary.com/v0";

        public static async Task<UrbanDictionaryDefinition[]> GetDefinition(string term)
        {
            var wc = new WebClient();
            var resp = await wc.DownloadStringTaskAsync($"{ApiBase}/define?term={WebUtility.UrlEncode(term)}");
            var obj = JsonConvert.DeserializeObject<GenericList<UrbanDictionaryDefinition>>(resp);
            return obj.List;
        }

        public class GenericList<T>
        {
            [JsonProperty("list")]
            public T[] List { get; set; }
        }

        public class UrbanDictionaryDefinition
        {
            [JsonProperty("word")]
            public string Word { get; set; }

            [JsonProperty("definition")]
            public string Definition { get; set; }

            [JsonProperty("example")]
            public string Example { get; set; }

            [JsonProperty("permalink")]
            public Uri Permalink { get; set; }

            [JsonProperty("defid")]
            public long DefinitionId { get; set; }

            [JsonProperty("sound_urls")]
            public object[] SoundUrls { get; set; }

            [JsonProperty("author")]
            public string Author { get; set; }

            [JsonProperty("written_on")]
            public DateTimeOffset WrittenOn { get; set; }

            [JsonProperty("current_vote")]
            public string CurrentVote { get; set; }

            [JsonProperty("thumbs_up")]
            public long ThumbsUp { get; set; }

            [JsonProperty("thumbs_down")]
            public long ThumbsDown { get; set; }
        }
    }
}
