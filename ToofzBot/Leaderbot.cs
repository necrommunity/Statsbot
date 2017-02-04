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

    public static class Leaderbot
    {
        public static List<LeaderboardInfo> lbInfo;
        public static Dictionary<string, string> nicks = new Dictionary<string, string>();

        public static void RegisterLeaderboards()
        {
            string xml = ApiSender.GetLeaderboard("", 0);
            List<LeaderboardInfo> list = ParseIndex();
            File.WriteAllText(@"Leaderboards.json", JsonConvert.SerializeObject(list, Newtonsoft.Json.Formatting.Indented));
        }

        public static void LoadNames()
        {
            string path = @"ProfileNames.json";
            if (!File.Exists(path))
                File.WriteAllText(path, JsonConvert.SerializeObject(new Dictionary<string, string>() { { 76561198112122029.ToString(), "Naymin" } }));
            nicks = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(path));
        }

        public static void CheckNames(List<Entry> ids)
        {
            List<Entry> toUpdate = new List<Entry>();
            string check;
            foreach(Entry e in ids)
            {
                check = "";
                if (nicks.TryGetValue(e.SteamId, out check))
                {
                    e.ProfileName = check;
                }
                else
                {
                    toUpdate.Add(e);
                }
            }
            if (!(toUpdate.Count == 0))
            {
                UpdateNames(toUpdate);
                CheckNames(toUpdate);
            }
        }

        public static void UpdateNames(List<Entry> id)
        {
            foreach (Entry e in id)
            {
                string cnick = ApiSender.GetSteamName(e.SteamId);
                nicks[e.SteamId] = cnick;
            }
            File.WriteAllText(@"ProfileNames.json", JsonConvert.SerializeObject(nicks, Newtonsoft.Json.Formatting.Indented));
        }

        public static List<LeaderboardInfo> ParseIndex()
        {
            string xml = ApiSender.GetLeaderboard("", 0);
            List<LeaderboardInfo> list = new List<LeaderboardInfo>();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            foreach (XmlNode n in doc.DocumentElement.ChildNodes)
            {
                if (n.Name == "leaderboard")
                {
                    LeaderboardInfo lb = new LeaderboardInfo();

                    foreach (XmlNode e in n)
                    {
                        switch (e.Name)
                        {
                            case ("lbid"):
                                lb.Id = e.InnerText;
                                break;
                            case ("entries"):
                                lb.EntryCount = int.Parse(e.InnerText);
                                break;
                            case ("display_name"): // there is probably a neat way to do this
                                string s = e.InnerText;
                                lb.DisplayName = s;
                                if (!s.Contains("Amplified"))
                                    lb.Leaderboard.Amplified = false;
                                if (s.Contains("Seeded"))
                                    lb.Leaderboard.Seeded = true;
                                for (int i = 0; i < 3; i++)
                                {
                                    if (s.Contains(Enum.GetNames(typeof(RunType))[i]))
                                    {
                                        lb.Leaderboard.Type = (RunType)i;
                                        break;
                                    }
                                }
                                for (int i = 0; i < 13; i++)
                                {
                                    if (s.Contains(Enum.GetNames(typeof(Character))[i]))
                                    {
                                        lb.Leaderboard.Char = (Character)i;
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

        public static List<Entry> ParseLeaderboard(string id, int offset)
        {
            string xml = ApiSender.GetLeaderboard(id, offset);
            List<Entry> list = new List<Entry>();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            foreach (XmlNode n in doc.DocumentElement.ChildNodes[8])
            {
                    Entry en = new Entry();

                    foreach (XmlNode e in n)
                    {
                        switch (e.Name)
                        {
                        case "steamid":
                            en.SteamId = e.InnerText;
                            break;
                        case "score":
                            en.Score = int.Parse(e.InnerText);
                            break;
                        case "rank":
                            en.Rank = int.Parse(e.InnerText);
                            break;
                        case "ugcid":
                            en.UgcId = e.InnerText;
                            break;
                        }
                    }

                    list.Add(en);
            }

            return list;
        }
    }
}
