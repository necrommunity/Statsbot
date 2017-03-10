using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Statsbot
{

    public enum Character { All, Aria, Bard, Bolt, Cadence, Coda, Diamond, Dorian, Dove, Eli, Melody, Monk, Nocturna, Story }
    public enum RunType { Deathless, Score, Speed }
    public enum Mode { Standard, Hard, NoReturn }

    public class Category
    {
        public bool Amplified { get; set; } = true;
        public bool Seeded { get; set; }
        public Character Char { get; set; }
        public RunType Type { get; set; }
        public Mode Mode { get; set; } = Mode.Standard;

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            Category compare = obj as Category;
            if (Amplified == compare.Amplified && Seeded == compare.Seeded && Char == compare.Char && Type == compare.Type && Mode == compare.Mode)
                return true;
            else
                return false;
        }

        public string GetRunString()
        {
            StringBuilder sb = new StringBuilder();
            if (Seeded)
                sb.Append("Seeded");
            sb.Append(Type.ToString());
            return (sb.ToString());
        }

        public override int GetHashCode()
        {
            return (Char.GetHashCode() + Type.GetHashCode() + Seeded.GetHashCode() + Amplified.GetHashCode());
        }
    }


}
