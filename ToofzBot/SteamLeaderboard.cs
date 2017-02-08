using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToofzBot
{

    public enum Character{ All, Aria, Bard, Bolt, Cadence, Coda, Dorian, Dove, Eli, Melody, Monk, Nocturna, Story }
    public enum RunType { Deathless, Score, Speed }

    public class SteamLeaderboard
    {
        public bool Amplified { get; set; } = true;
        public bool Seeded { get; set; }
        public Character Char { get; set; }
        public RunType Type { get; set; }

        public string ID { get; set; }
        public int EntryCount { get; set; }
        public string DisplayName { get; set; }
    }
}
