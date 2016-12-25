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

        public static void WriteConfig(Config config)
        {
            File.WriteAllText(@"Config.json", JsonConvert.SerializeObject(config, Formatting.Indented));
        }

        public Config()
        {
            SteamKey = "";
            DiscordToken = "";
        }

        public static Config ReadConfig()
        {
            if (!File.Exists(@"Config.json"))
                WriteConfig(new Config());
            return( JsonConvert.DeserializeObject<Config>(File.ReadAllText(@"Config.json")));
        }
    }
}
