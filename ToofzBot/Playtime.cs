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

        public static PlaytimeResponse GetResponse(string id) // https://developer.valvesoftware.com/wiki/Steam_Web_API#GetOwnedGames_.28v0001.29
        {
            string response;
            using (WebClient client = new WebClient())
            {
                response = client.DownloadString("http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key=" + Program.config.SteamKey + "&steamid=" + id);
            }
            PlaytimeResponse toReturn = JsonConvert.DeserializeObject<SteamResponse>(response).Response;
            return (toReturn);
        }
    }
    public class PlaytimeResponse
    {
        public Playtime[] Games { get; set; }

        public static Playtime GetPlaytime(string id)
        {
            PlaytimeResponse response = SteamResponse.GetResponse(id);
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
