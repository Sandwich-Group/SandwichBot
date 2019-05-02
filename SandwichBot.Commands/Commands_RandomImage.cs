using System.Net;
using HoLLy.DiscordBot.Commands;
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
    }
}
