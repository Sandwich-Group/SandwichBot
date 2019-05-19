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
    }
}
