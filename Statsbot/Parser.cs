using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Xml;
using System.Net;
using Newtonsoft.Json;

namespace ToofzBot
{
    public static class Parser
    {

        public static string LeaderboardGet(string id)
        {
            string response = "";
            try
            {
                using (WebClient client = new WebClient())
                {
                    if (id == "")
                        response = client.DownloadString("http://steamcommunity.com/stats/247080/leaderboards/?xml=1");
                    else
                        response = client.DownloadString("http://steamcommunity.com/stats/247080/leaderboards/" + id + "/?xml=1");
                }
            }
            catch
            {
                Console.WriteLine("Serious error occured. Server not responding.");
            }
            return response;
        }

        public static void RegisterLeaderboards()
        {
            string xml = LeaderboardGet("");
            List<SteamLeaderboard> list = ParseLbId(xml);
            File.WriteAllText(@"SteamLeaderboards.json", JsonConvert.SerializeObject(list, Newtonsoft.Json.Formatting.Indented));
        }

        public static List<SteamLeaderboard> ParseLbId(string xml)
        {
            List<SteamLeaderboard> list = new List<SteamLeaderboard>();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            foreach (XmlNode n in doc.DocumentElement.ChildNodes)
            {

                if (n.Name == "leaderboard")
                {

                    SteamLeaderboard lb = new SteamLeaderboard();

                    foreach (XmlNode e in n)
                    {
                        switch (e.Name)
                        {
                            case ("lbid"):
                                lb.ID = e.InnerText;
                                break;
                            case ("entries"):
                                lb.EntryCount = int.Parse(e.InnerText);
                                break;
                            case ("display_name"): // there is probably a neat way to do this
                                string s = e.InnerText;
                                lb.DisplayName = s;
                                if (!s.Contains("Amplified"))
                                    lb.Amplified = false;
                                if (s.Contains("Seeded"))
                                    lb.Seeded = true;
                                for (int i = 0; i < 3; i++)
                                {
                                    if (s.Contains(Enum.GetNames(typeof(RunType))[i]))
                                    {
                                        lb.Type = (RunType)i;
                                        break;
                                    }
                                }
                                for (int i = 0; i < 13; i++)
                                {
                                    if (s.Contains(Enum.GetNames(typeof(Character))[i]))
                                    {
                                        lb.Char = (Character)i;
                                        break;
                                    }
                                }
                                break;
                        }
                    }

                    list.Add(lb);
                }
            }

            return list;
        }
    }
}
