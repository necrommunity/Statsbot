using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToofzBot
{

    public class SteamUser
    {
        public string Steamid { get; set; }
        public string Personaname { get; set; }
    }

    public class SteamUsers
    {
        public SteamUser[] Players { get; set; }
    }

    public class SteamResponse
    {
        public SteamUsers Response { get; set; }
    }
}