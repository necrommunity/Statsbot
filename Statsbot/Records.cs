using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Statsbot
{

    public class RecordsResponse
    {
        public Records Playerstats { get; set; }
    }

    public class Records
    {
        public static Dictionary<string, string> Names { get; set; } = JsonConvert.DeserializeObject<Dictionary<string, string>>(System.IO.File.ReadAllText(@"Stats.json"));

        public string SteamID { get; set; }
        public Stat[] Stats { get; set; }
        public Dictionary<string, int> Organized { get; set; } = new Dictionary<string, int>();

        public void Organize()
        {
            foreach (string s in Names.Values)
            {
                Organized[s] = 0;
            }
            foreach (Stat s in Stats)
            {
                if (Names.ContainsKey(s.Name))
                    Organized[Names[s.Name]] = s.Value;
            }
        }
    }

    public class Stat
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }


}
