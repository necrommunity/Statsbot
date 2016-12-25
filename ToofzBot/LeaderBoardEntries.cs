using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToofzBot
{
    public class LeaderBoardEntries
    {
        public Run Run { get; set; }
        //public DateTime LastUpdate { get; set; }
        //public DateTime Timestamp { get; set; }
        //public int RecordsFiltered { get; set; }
        public int RecordsTotal { get; set; }
        public int Page { get; set; }
        public Entry[] Entries { get; set; }   
    }

}
