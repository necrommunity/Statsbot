using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Statsbot
{
    public enum Character { All, DLC, Aria, Bard, Bolt, Cadence, Coda, Diamond, Dorian, Dove, Eli, Mary, Melody, Monk, Nocturna, Story, Tempo }
    public enum RunType { Deathless, Score, Speed }
    public enum Mode { Standard, Hardmode, NoReturn, Phasing, Randomizer, Mystery }
    public enum Product { Classic, Amplified }

    public class Leaderboard
    {
        public string ID { get; set; }
        public int EntryCount { get; set; }
        public string DisplayName { get; set; }
        public Category Category { get; set; } = new Category();
    }

}
