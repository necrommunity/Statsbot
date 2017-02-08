using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToofzBot
{
    public class Entry
    {
        public Leaderboard Leaderboard { get; set; }
        public Player Player { get; set; }
        public string SteamId { get; set; }
        public int Rank { get; set; }
        public int Score { get; set; }
        
    }
}
