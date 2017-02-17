using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ToofzBot
{
    public class SteamResponse
    {
        public PlaytimeResponse Response { get; set; }
    }

    public class PlaytimeResponse
    {
        public Playtime[] Games { get; set; }

        public static Playtime GetPlaytime(string id)
        {
            PlaytimeResponse response = ApiSender.GetPlaytime(id);
            foreach (Playtime t in response.Games)
            {
                if (t.Appid == 247080)
                    return t;
            }
            return null;
        }
    }

    public class Playtime
    {
        public int Appid { get; set; } //247080 for nd
        public int Playtime_forever { get; set; }
        public int Playtime_2weeks { get; set; }
    }


}
