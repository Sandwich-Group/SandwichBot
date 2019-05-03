using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace HoLLy.DiscordBot.Sandwich.Tools
{
    public static class DuckDuckGoNoApi
    {
        private const string ApiBase = "https://duckduckgo.com/";
        private static readonly Regex RegexVqdToken = new Regex("vqd='([^']+)");

        public static async Task<Uri> SearchImage(string query)
        {
            var wc = new WebClient();
            
            var searchPage = await wc.DownloadStringTaskAsync(ApiBase + $"?q={HttpUtility.UrlEncode(query)}&t=h_&iax=images&ia=images");
            var vdq = RegexVqdToken.Match(searchPage).Groups[1].Value;

            var searchResult = await wc.DownloadStringTaskAsync(ApiBase + "i.js" + $"?l=wt-wt&o=json&q=box&vqd={vdq}&f=,&p=1");
            var resp = JsonConvert.DeserializeObject<DdgImageResponse>(searchResult);
            return resp.Results.First().Image;
        }
        
        public class DdgImageResponse
        {
            [JsonProperty("response_type")]
            public string ResponseType { get; set; }

            [JsonProperty("query")]
            public string Query { get; set; }

            [JsonProperty("ads")]
            public object Ads { get; set; }

            [JsonProperty("vqd")]
            public Vqd Vqd { get; set; }

            [JsonProperty("results")]
            public Result[] Results { get; set; }

            [JsonProperty("next")]
            public string Next { get; set; }
        }

        public class Result
        {
            [JsonProperty("thumbnail")]
            public Uri Thumbnail { get; set; }

            [JsonProperty("url")]
            public Uri Url { get; set; }

            [JsonProperty("height")]
            public long Height { get; set; }

            [JsonProperty("width")]
            public long Width { get; set; }

            [JsonProperty("image")]
            public Uri Image { get; set; }

            [JsonProperty("title")]
            public string Title { get; set; }

            [JsonProperty("source")]
            public Source Source { get; set; }
        }

        public class Vqd
        {
            [JsonProperty("box")]
            public string Box { get; set; }
        }

        public enum Source { Yahoo }
    }
}