using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToofzBot
{

    public enum Character{ All, Aria, Bard, Bolt, Cadence, Coda, Dorian, Dove, Eli, Melody, Monk, Nocturna, Story }
    public enum RunType { Deathless, Score, Speed }

    public class Leaderboard
    {
        public bool Amplified { get; set; } = true;
        public bool Seeded { get; set; }
        public Character Char { get; set; }
        public RunType Type { get; set; }

        public bool isEqual(Leaderboard lb)
        {
            if (Amplified == lb.Amplified && Seeded == lb.Seeded && Char == lb.Char && Type == lb.Type)
                return true;
            else
                return false;
        }
    }

    
}
