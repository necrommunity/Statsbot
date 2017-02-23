using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Statsbot
{
    public class Leaderboard
    {
        public string ID { get; set; }
        public int EntryCount { get; set; }
        public string DisplayName { get; set; }
        public Category Category { get; set; } = new Category();
    }

}
