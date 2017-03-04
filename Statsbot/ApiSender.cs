using System;
using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Text;

namespace Statsbot
{
    public static class ApiSender
    {

        public static T ServerGet<T>(string args, string product)
        {

            T obj = default(T);
            string request = request = "http://api.steampowered.com/" + args + "&key=" + Program.config.SteamKey;

            if (product == "toofz")
                request = "https://api.toofz.com/" + args;

            string response;
            try
            {
                using (WebClient client = new WebClient())
                {
                    response = client.DownloadString(request);
                }
                obj = JsonConvert.DeserializeObject<T>(response);
            }
            catch
            {
                Console.WriteLine("[Server not responding.]");
            }
            return obj;
        }

        public static PlayerNames GetNames(string q)
        {
            PlayerNames a = ServerGet<PlayerNames>("players?q=" + q, "toofz");
            return (a);
        }

        public static PlayerEntries GetPlayerScores(string id)
        {
            return (ServerGet<PlayerEntries>("players/" + id + "/entries", "toofz"));
        }

        public static Records GetPlayerRecords(string id) // https://developer.valvesoftware.com/wiki/Steam_Web_API#GetUserStatsForGame_.28v0002.29
        {
            try
            {
                return (ServerGet<RecordsResponse>("ISteamUserStats/GetUserStatsForGame/v0002/?appid=247080&steamid=" + id, "steam").Playerstats);
            }
            catch
            {
                return (new Records());
            }
        }

        public static PlaytimeResponse GetPlaytime(string id) // https://developer.valvesoftware.com/wiki/Steam_Web_API#GetOwnedGames_.28v0001.29
        {
            return (ServerGet<SteamPlaytimeResponse>("IPlayerService/GetOwnedGames/v0001/?steamid=" + id, "steam").Response);
        }

        public static string GetLeaderboard(string id, int offset) // https://partner.steamgames.com/documentation/community_data
        {
            string response;
            try
            {
                using (WebClient client = new WebClient())
                {
                    if (id == "")
                        response = client.DownloadString("http://steamcommunity.com/stats/247080/leaderboards/?xml=1");
                    else
                        response = client.DownloadString("http://steamcommunity.com/stats/247080/leaderboards/" + id + "/?xml=1&start=" + offset + "&end=" + (offset + 9));
                }
            }
            catch
            {
                Console.WriteLine("Server not responding.");
                return ("");
            }
            return response;
        }

        public static void GetSteamNames(List<Entry> ids) // https://developer.valvesoftware.com/wiki/Steam_Web_API#GetPlayerSummaries_.28v0002.29
        {
            //searches for the profile names of given ids
            ids = ids.OrderBy(Entry => Entry.Steamid).ToList();
            SteamUser[] steamUsers;

            StringBuilder request = new StringBuilder();
            foreach (Entry e in ids) { request.Append(e.Steamid + ","); }
            steamUsers = ServerGet<SteamUsersResponse>("ISteamUser/GetPlayerSummaries/v0002/?steamids=" + request.ToString(), "steam").Response.Players;
            steamUsers = steamUsers.OrderBy(SteamUser => SteamUser.Steamid).ToArray();
            for (int i = 0; i < ids.Count; i++)
            {
                ids[i].Personaname = steamUsers[i].Personaname;
            }
        }
    }
}

