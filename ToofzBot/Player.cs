using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToofzBot
{
    public class Player
    {
        public string ID { get; set; }
        public string Display_name { get; set; }
        public string Avatar { get; set; }
    }

    public class PlayerResults
    {
        public Player[] Players { get; set; }
    }
}
