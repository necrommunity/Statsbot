using System;
using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

namespace ToofzBot
{
    public static class ApiSender
    {

        public static T ServerGet<T>(string args)
        {
            T obj;
            string response;
            using (WebClient client = new WebClient())
            {
                response = client.DownloadString("https://api.toofz.com/" + args);
            }
            obj = JsonConvert.DeserializeObject<T>(response);
            return obj;
        }

        public static void RegisterLeaderboards()
        {
            Leaderboard[] response = ServerGet<Leaderboard[]>("leaderboards/primaries");

            File.WriteAllText(@"Leaderboards.json", JsonConvert.SerializeObject(response, Formatting.Indented));
            Console.WriteLine("[Leaderboards registered to Leaderboards.json]");
        }

        public static Leaderboard[] GetLeaderboards()
        {
            if (!File.Exists(@"Leaderboards.json"))
            {
                RegisterLeaderboards();
            }
            return (JsonConvert.DeserializeObject<Leaderboard[]>(File.ReadAllText(@"Leaderboards.json")));
        }

        public static PlayerResults GetPlayers(string q)
        {
            return (ServerGet<PlayerResults>("players?q=" + q));
        }

        public static Player GetPlayersId(string steamId)
        {
            return (ServerGet<Player>("players/" + steamId));
        }
        
        public static LeaderBoardEntries GetLeaderboardEntries(int lbId, int offset)
        {
            string lboard = "leaderboards/" + lbId + "/entries?offset=" + offset;
            return (ServerGet<LeaderBoardEntries>(lboard));
        }

        public static SteamUser GetSteamUser(string id) //https://developer.valvesoftware.com/wiki/Steam_Web_API#GetPlayerSummaries_.28v0002.29
        {
            SteamResponse users;
            string response;
            using (WebClient client = new WebClient())
            {
                response = client.DownloadString("http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=" + Program.config.SteamKey +"&steamids=" + id);
            }
            users = JsonConvert.DeserializeObject<SteamResponse>(response);
            return users.Response.Players[0];
        }
    }
}

