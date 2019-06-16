using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HoLLy.DiscordBot.Sandwich.Tools
{
    public static class ExchangeRate
    {
        // TODO: switch to using https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml
        // see https://github.com/exchangeratesapi/exchangeratesapi/issues/44
        private const string ApiBase = "https://api.exchangeratesapi.io/latest";

        public static async Task<double> ConvertCurrency(double amount, string from, string to = "EUR") => await GetExchangeRate(@from, to) * amount;

        public static async Task<double> GetExchangeRate(string from, string to = "EUR")
        {
            from = WebUtility.UrlEncode(from.ToUpperInvariant());
            to = WebUtility.UrlEncode(to.ToUpperInvariant());

            if (from?.Length != 3) throw new Exception("Base currency invalid");
            if (to?.Length != 3) throw new Exception("Target currency invalid");

            string url = ApiBase + $"?base={from}&symbols={to}";
            string responseString = await new WebClient().DownloadStringTaskAsync(url);
            var response = JsonConvert.DeserializeObject<Stuff>(responseString);

            if (!string.IsNullOrEmpty(response.Error))
                throw new Exception(response.Error);

            return response.Rates[to];
        }

        public class Stuff
        {
            [JsonProperty("base")]
            public string Base { get; set; }

            [JsonProperty("rates")]
            public Dictionary<string, double> Rates { get; set; }

            [JsonProperty("date")]
            public DateTimeOffset Date { get; set; }

            [JsonProperty("error")]
            public string Error { get; set; }
        }
    }
}
