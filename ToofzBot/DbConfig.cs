using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace ToofzBot
{
    public class DbConfig
    {
        public string UserID { get; set; }
        public string Password { get; set; }
        public string Server { get; set; }
        public string Database { get; set; }

        public DbConfig()
        {
            UserID = "";
            Password = "";
            Server = "";
            Database = "";
        }

        public static DbConfig ReadConfig()
        {
            if (!File.Exists(@"DbConfig.json"))
            {
                Console.WriteLine("DbConfig.json created.");
                File.WriteAllText(@"DbConfig.json", JsonConvert.SerializeObject(new DbConfig(), Formatting.Indented));
            }
            return (JsonConvert.DeserializeObject<DbConfig>(File.ReadAllText(@"DbConfig.json")));
        }

        public override string ToString()
        {
            return ("server=" + Server + ";" + "database=" + Database + ";" + "uid=" + UserID + ";" + "pwd=" + Password + ";");
        }
    }
}
