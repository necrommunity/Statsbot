using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToofzBot
{
    public class PlayerResult
    {
        public string SteamId { get; set; }
        public string Name { get; set; }
        public int EntryCount { get; set; }
        public BestEntry BestEntry { get; set; }
    }

    public class PlayerResults
    {
        public PlayerResult[] Entries { get; set; }
    }
}
