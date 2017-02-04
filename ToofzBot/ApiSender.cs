using System;
using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

namespace ToofzBot
{
    public static class ApiSender
    {

        public static string GetLeaderboard(string id, int offset) // https://partner.steamgames.com/documentation/community_data
        {
            string response = "";
            try
            {
                using (WebClient client = new WebClient())
                {
                    if (id == "")
                        response = client.DownloadString("http://steamcommunity.com/stats/247080/leaderboards/?xml=1");
                    else
                        response = client.DownloadString("http://steamcommunity.com/stats/247080/leaderboards/" + id + "/?xml=1&start=" + offset + "&end=" + (offset + 14));
                }
            }
            catch
            {
                Console.WriteLine("Serious error occured. Server not responding.");
            }
            return response;
        }


        public static string GetSteamName(string id) // https://developer.valvesoftware.com/wiki/Steam_Web_API#GetPlayerSummaries_.28v0002.29
        {
            SteamResponse users;
            string response;
            using (WebClient client = new WebClient())
            {
                response = client.DownloadString("http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=" + Program.config.SteamKey + "&steamids=" + id);
            }
            users = JsonConvert.DeserializeObject<SteamResponse>(response);
            return users.Response.Players[0].Personaname;
        }
    }
}

