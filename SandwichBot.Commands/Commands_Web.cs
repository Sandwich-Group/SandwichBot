using System.Net;
using HoLLy.DiscordBot.Commands;
using HoLLy.DiscordBot.Sandwich.Tools;
using Newtonsoft.Json;

namespace HoLLy.DiscordBot.Sandwich
{
    public static partial class Commands
    {
        [Command("cat", "Nya!")]
        public static string RandomCat() => ((dynamic)JsonConvert.DeserializeObject(new WebClient().DownloadString("http://aws.random.cat/meow"))).file;

        [Command("dog", "Woof!")]
        public static string RandomDog() => ((dynamic)JsonConvert.DeserializeObject(new WebClient().DownloadString("https://random.dog/woof.json"))).url;

        [Command("fox", "Floof!")]
        public static string RandomFox() => ((dynamic)JsonConvert.DeserializeObject(new WebClient().DownloadString("https://randomfox.ca/floof/"))).image;

        [Command("ud", "Gets a definition from Urban Dictionary")]
        public static string UrbanDictionary(string term)
        {
            var list = Tools.UrbanDictionary.GetDefinition(term).Result;
            if (list.Length == 0)
                return "No definition found.";

            var def = list[0];
            return $"{removeLinks(def.Definition.Escape())}\n> *{removeLinks(def.Example.Escape())}*";

            string removeLinks(string s) => s.Replace("[", string.Empty).Replace("]", string.Empty);
        }

        [Command("img", "Searches for an image on DuckDuckGo")]
        public static string ImageSearch(string query) => DuckDuckGoNoApi.SearchImage(query).Result.ToString();

        [Command("cur", "Converts a currency")]
        public static string ConvertCurrency(string parameter)
        {
            if (parameter.IndexOf(' ') == -1)
                return usage();    // TODO: try 1.23USD

            string firstPart = parameter.Substring(0, parameter.IndexOf(' '));
            string secondPart = parameter.Substring(parameter.IndexOf(' ') + 1);
            string to, from;

            if (double.TryParse(firstPart, out double value)) {
                from = secondPart;
                to = "EUR";
            } else {
                to = firstPart;
                if (secondPart.IndexOf(' ') == -1)
                    return usage();    // TODO: try 1.23USD

                if (!double.TryParse(secondPart.Substring(0, secondPart.IndexOf(' ')), out value))
                    return usage();

                from = secondPart.Substring(secondPart.IndexOf(' ') + 1);
            }

            // TODO: convert symbols?

            return $"{value} {from} = {ExchangeRate.ConvertCurrency(value, from, to).Result} {to}";

            string usage() => "Parameter format: `<amount> `<from>` or `<to> <amount> <from>`";
        }
    }
}
