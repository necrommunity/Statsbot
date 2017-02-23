using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Statsbot
{
    public class PlayerEntries
    {
        public Player Player { get; set; }
        public int Total { get; set; }
        public PlayerEntry[] Entries { get; set; }
    }

    public class PlayerEntry
    {
        public ToofzBoard Leaderboard { get; set; }
        public Player Player { get; set; }
        public string SteamID { get; set; }
        public int Rank { get; set; }
        public int Score { get; set; }

    }
}
