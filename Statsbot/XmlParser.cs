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

namespace Statsbot
{

    public static class XmlParser
    {

        public static Dictionary<Category, Leaderboard> lbInfo;

        public static void RegisterLeaderboards(Dictionary<Category, Leaderboard> lbInfo)
        {
            File.WriteAllText(@"Leaderboards.json", JsonConvert.SerializeObject(lbInfo, Newtonsoft.Json.Formatting.Indented));
        }

        public static Dictionary<Category, Leaderboard> ParseIndex()
        {
            string xml = ApiSender.GetLeaderboard("", 0);
            Dictionary<Category, Leaderboard> list = new Dictionary<Category, Leaderboard>();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            foreach (XmlNode n in doc.DocumentElement.ChildNodes)
            {
                if (n.Name == "leaderboard")
                {
                    Leaderboard lb = new Leaderboard();

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
                            case ("display_name"):
                                string s = e.InnerText;
                                lb.DisplayName = s;
                                if (!s.Contains("Amplified"))
                                    lb.Category.Amplified = false;
                                if (s.Contains("Seeded"))
                                    lb.Category.Seeded = true;
                                if (s.Contains("Hard"))
                                    lb.Category.Mode = Mode.Hard;
                                if (s.Contains("Return"))
                                    lb.Category.Mode = Mode.NoReturn;
                                for (int i = 0; i < 3; i++)
                                {
                                    if (s.Contains(Enum.GetNames(typeof(RunType))[i]))
                                    {
                                        lb.Category.Type = (RunType)i;
                                        break;
                                    }
                                }
                                for (int i = 0; i < 14; i++)
                                {
                                    if (s.Contains(Enum.GetNames(typeof(Character))[i]))
                                    {
                                        lb.Category.Char = (Character)i;
                                        break;
                                    }
                                }
                                break;
                        }
                    }

                    list.Add(lb.Category, lb);
                }
            }
            RegisterLeaderboards(list);
            return list;
        }

        public static List<Entry> ParseLeaderboard(Leaderboard lb, int offset)
        {
            string xml = ApiSender.GetLeaderboard(lb.ID, offset);
            List<Entry> list = new List<Entry>();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            lbInfo[lb.Category].EntryCount = Convert.ToInt32(doc.DocumentElement.ChildNodes[3].InnerText);

            int i = 7;
            if (doc.DocumentElement.ChildNodes[6].Name == "nextRequestURL")
                i = 8;

            foreach (XmlNode n in doc.DocumentElement.ChildNodes[i])
            {
                Entry en = new Entry();

                foreach (XmlNode e in n)
                {
                    switch (e.Name)
                    {
                        case "steamid":
                            en.Steamid = e.InnerText;
                            break;
                        case "score":
                            en.Score = int.Parse(e.InnerText);
                            break;
                        case "rank":
                            en.Rank = int.Parse(e.InnerText);
                            break;
                        case "ugcid":
                            en.UgcID = e.InnerText;
                            break;
                    }
                }

                list.Add(en);
            }
            return list;
        }
    }
}
