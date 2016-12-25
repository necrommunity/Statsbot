using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToofzBot
{
    public class Player
    {
        public DateTime LeaderboardsLastUpdate { get; set; }
        public DateTime Timestamp { get; set; }
        public PlayerRun[] Runs { get; set; }
    }
}
