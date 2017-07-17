using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace Statsbot
{
    public class Config
    {
        public string SteamKey { get; set; }
        public string DiscordToken { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public Config()
        {
            SteamKey = "";
            DiscordToken = "";
            Username = "";
            Password = "";
        }

        public static Config ReadConfig()
        {
            if (!File.Exists(@"Config.json"))
            {
                Console.WriteLine("Config.json created. Please fill in the keys.");
                File.WriteAllText(@"Config.json", JsonConvert.SerializeObject(new Config(), Formatting.Indented));
            }
            return (JsonConvert.DeserializeObject<Config>(File.ReadAllText(@"Config.json")));
        }
    }
}
