using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToofzBot
{
    public class LeaderboardInfo
    {
        public string Id { get; set; }
        public int EntryCount { get; set; }
        public string DisplayName { get; set; }
        public Leaderboard Leaderboard { get; set; } = new Leaderboard();
    }
}
