using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToofzBot
{
    public class LeaderboardEntries
    {
        public Leaderboard Leaderboard { get; set; }
        public int Total { get; set; }
        public Entry[] Entries { get; set; }
    }
}
