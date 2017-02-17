using System;
using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

namespace ToofzBot
{
    public static class ApiSender
    {

        public static T ServerGet<T>(string args, string product)
        {

            T obj = default(T);
            string request = "https://api.toofz.com/" + args;

            if (product == "steam")
                request = "http://api.steampowered.com/" + args + "&key=" + Program.config.SteamKey;

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

        public static LeaderboardResults GetLeaderboardID(string character, string type, bool amplified)
        {
            string product = "classic";
            if (amplified)
                product = "amplified";
            return (ServerGet<LeaderboardResults>("leaderboards?products=" + product + "&runs=" + type + "&characters=" + character, "toofz"));
        }

        public static LeaderboardEntries GetLeaderboardEntries(int id, int offset)
        {
            string lboard = "leaderboards/" + id + "/entries?offset=" + offset;
            return (ServerGet<LeaderboardEntries>(lboard, "toofz"));
        }

        public static PlayerResults GetNames(string q)
        {
            return (ServerGet<PlayerResults>("players?q=" + q, "toofz"));
        }

        public static PlayerEntries GetPlayerScores(string id)
        {
            return (ServerGet<PlayerEntries>("players/" + id + "/entries", "toofz"));
        }

        public static PlayerStats GetPlayerStats(string id) // https://developer.valvesoftware.com/wiki/Steam_Web_API#GetUserStatsForGame_.28v0002.29
        {
            try
            {
                return (ServerGet<StatsResponse>("ISteamUserStats/GetUserStatsForGame/v0002/?appid=247080&steamid=" + id, "steam").Playerstats);
            }
            catch
            {
                return (new PlayerStats());
            }
        }

        public static PlaytimeResponse GetPlaytime(string id) // https://developer.valvesoftware.com/wiki/Steam_Web_API#GetOwnedGames_.28v0001.29
        {
            PlaytimeResponse a = ServerGet<SteamResponse>("IPlayerService/GetOwnedGames/v0001/?steamid=" + id, "steam").Response;
            return (a);
        }

    }
}

