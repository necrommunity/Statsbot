using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Statsbot
{
    public class ToofzBoard
    {
        public int ID { get; set; }
        public string Product { get; set; }
        public string Character { get; set; }
        public string Run { get; set; }
        public string Mode { get; set; }

        public Category ToCategory()
        {
            Category cat = new Category();
            if (Product == "classic")
                cat.Product = Statsbot.Product.Classic;
            for (int i = 0; i < 17; i++)
            {
                switch (i)
                {
                    case 0:
                        if (Character == "all-characters")
                            cat.Char = Statsbot.Character.All;
                        break;
                    case 1:
                        if (Character == "all-characters-amplified")
                            cat.Char = Statsbot.Character.DLC;
                        break;
                    case 15:
                        if (Character == "story-mode")
                            cat.Char = Statsbot.Character.Story;
                        break;
                    default:
                        if (Character.Equals(Enum.GetNames(typeof(Character))[i], StringComparison.InvariantCultureIgnoreCase))
                            cat.Char = (Character)i;
                        break;
                }
            }
            switch (Run)
            {
                case "score":
                    cat.Type = RunType.Score;
                    break;
                case "seeded-score":
                    cat.Type = RunType.Score;
                    cat.Seeded = true;
                    break;
                case "speed":
                    cat.Type = RunType.Speed;
                    break;
                case "seeded-speed":
                    cat.Type = RunType.Speed;
                    cat.Seeded = true;
                    break;
                case "deathless":
                    cat.Type = RunType.Deathless;
                    break;
            }
            switch (Mode)
            {
                case "standard":
                    cat.Mode = Statsbot.Mode.Standard;
                    break;
                case "no-return":
                    cat.Mode = Statsbot.Mode.NoReturn;
                    break;
                case "hard":
                    cat.Mode = Statsbot.Mode.Hardmode;
                    break;
                case "mystery":
                    cat.Mode = Statsbot.Mode.Mystery;
                    break;
                case "phasing":
                    cat.Mode = Statsbot.Mode.Phasing;
                    break;
                case "randomizer":
                    cat.Mode = Statsbot.Mode.Randomizer;
                    break;
            }
            return cat;
        }
    }
}
