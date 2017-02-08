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
            T obj = default(T);
            string response;
            try
            {
                using (WebClient client = new WebClient())
                {
                    response = client.DownloadString("https://api.toofz.com/" + args);
                }
                obj = JsonConvert.DeserializeObject<T>(response);
            }
            catch
            {
                Console.WriteLine("Serious error occured. Server not responding.");
            }
            return obj;
        }

        public static LeaderboardResults GetLeaderboardId(string character, string type, bool amplified)
        {
            string product = "classic";
            if (amplified)
                product = "amplified";
            return (ServerGet<LeaderboardResults>("leaderboards?products=" + product + "&runs=" + type + "&characters=" + character));
        }

        public static Leaderboard[] GetLeaderboards()
        {
            if (!File.Exists(@"Leaderboards.json"))
            {
                RegisterLeaderboards();
            }
            return (JsonConvert.DeserializeObject<Leaderboard[]>(File.ReadAllText(@"Leaderboards.json")));
        }

        public static LeaderboardEntries GetLeaderboardEntries(int lbId, int offset)
        {
            string lboard = "leaderboards/" + lbId + "/entries?offset=" + offset;
            return (ServerGet<LeaderboardEntries>(lboard));
        }

        public static void RegisterLeaderboards()
        {
            Leaderboard[] response = ServerGet<Leaderboard[]>("leaderboards/primaries");

            File.WriteAllText(@"Leaderboards.json", JsonConvert.SerializeObject(response, Formatting.Indented));
            Console.WriteLine("[Leaderboards registered to Leaderboards.json]");
        }

        public static PlayerResults GetPlayerResults(string q)
        {
            return (ServerGet<PlayerResults>("players?q=" + q));
        }

        public static PlayerEntries GetPlayerEntries(string id)
        {
            return (ServerGet<PlayerEntries>("players/" + id + "/entries"));
        }


    }
}

