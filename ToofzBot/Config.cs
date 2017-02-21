using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace ToofzBot
{
    public class Config
    {
        public string SteamKey { get; set; }
        public string DiscordToken { get; set; }

        public Config()
        {
            SteamKey = "";
            DiscordToken = "";
        }

        public static Config ReadConfig()
        {
            if (!File.Exists(@"Config.json"))
            {
                Console.WriteLine("Config.json created.");
                File.WriteAllText(@"Config.json", JsonConvert.SerializeObject(new Config(), Formatting.Indented));
            }
            return (JsonConvert.DeserializeObject<Config>(File.ReadAllText(@"Config.json")));
        }
    }
}
