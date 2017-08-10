using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace Statsbot
{
    public class Database
    {
        public string UserID { get; set; }
        public string Password { get; set; }
        public string Server { get; set; }
        public string Name { get; set; }

        public Database()
        {
            UserID = "";
            Password = "";
            Server = "";
            Name = "";
        }

        public static Database ReadConfig()
        {
            if (!File.Exists(@"Database.json"))
            {
                Console.WriteLine("Database.json created. Please enter the database's credentials.");
                File.WriteAllText(@"Database.json", JsonConvert.SerializeObject(new Database(), Formatting.Indented));
            }
            return (JsonConvert.DeserializeObject<Database>(File.ReadAllText(@"Database.json")));
        }

        public override string ToString()
        {
            return ("Server=" + Server + ";Database=" + Name + ";Uid=" + UserID + ";Pwd=" + Password + ";");
        }
    }
}
