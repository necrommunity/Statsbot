using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToofzBot
{
    public class RacerData
    {
        public string RaceID { get; set; }
        public RaceData RaceData { get; set; } = new RaceData();
        //public int DiscordID { get; set; }
        public string Time { get; set; }
        public string Rank { get; set; }
        public string Igt { get; set; }
        public string Comment { get; set; }
        public string Level { get; set; }
    }
}
