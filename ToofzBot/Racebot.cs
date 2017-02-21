using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace ToofzBot
{
    public class Racebot
    {
        private MySqlConnection Connection { get; set; }

        public Racebot()
        {
            DbConfig config = DbConfig.ReadConfig();
            if (config.Server == "")
            {
                Console.WriteLine("Please enter the database's credentials.");
                return;
            }
            Connection = new MySqlConnection(config.ToString());
        }

        public Dictionary<string, RacerData> GetRaces(string id, int offset)
        {
            Dictionary<string, RacerData> results = new Dictionary<string, RacerData>();
            using (Connection)
            {
                try
                {
                    Connection.Open();
                    MySqlCommand comm = new MySqlCommand("SELECT * FROM racer_data WHERE discord_id=" + id + " ORDER BY race_id DESC", Connection);
                    using (MySqlDataReader r = comm.ExecuteReader())
                    {
                        for (int i = 0; i < offset + 10 && r.Read(); i++)
                        {
                            if (i >= offset)
                            {
                                RacerData rd = new RacerData();
                                rd.RaceID = r[0].ToString();
                                rd.Time = r[2].ToString();
                                rd.Rank = r[3].ToString();
                                rd.Igt = r[4].ToString();
                                rd.Comment = r[5].ToString();
                                rd.Level = r[6].ToString();
                                results.Add(rd.RaceID, rd);
                            }
                        }
                    }
                    if (results.Keys.Count == 0)
                        return results;
                    StringBuilder races = new StringBuilder(" race_id=" + results.Keys.ElementAt(0));
                    for (int i = 1; i < results.Keys.Count; i++)
                    {
                        races.Append(" OR race_id=" + results.Keys.ElementAt(i));
                    }
                    comm = new MySqlCommand("SELECT * FROM race_data WHERE" + races.ToString() + " ORDER BY race_id DESC", Connection);
                    using (MySqlDataReader r = comm.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            results[r[0].ToString()].RaceData.Timestamp = r[1].ToString();
                            results[r[0].ToString()].RaceData.Character = r[2].ToString();
                            results[r[0].ToString()].RaceData.Descriptor = r[3].ToString();
                            results[r[0].ToString()].RaceData.Seed = r[5].ToString();
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return results;
        }

        public string GetDiscordID(string name)
        {
            string id = "";
            using (Connection)
            {
                try
                {
                    Connection.Open();
                    MySqlCommand comm = new MySqlCommand("SELECT * FROM user_data WHERE name='" + name + "'", Connection);
                    using (MySqlDataReader r = comm.ExecuteReader())
                    {
                        r.Read();
                        id = r[0].ToString();
                    }
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return id;
        }

        public string GetDiscordName(string id)
        {
            string name = "";
            using (Connection)
            {
                try
                {
                    Connection.Open();
                    MySqlCommand comm = new MySqlCommand("SELECT * FROM user_data WHERE discord_id=" + id, Connection);
                    using (MySqlDataReader r = comm.ExecuteReader())
                    {
                        r.Read();
                        name = r[1].ToString();
                    }
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return name;
        }

        public string DisplayResults(string user, string q)
        {

            int offset = 0;
            if (q.Contains("offset"))
            {
                q = q.Replace("offset", "&");
                q = q.Replace("=", null);
            }

            if (q.Contains("&"))
            {
                bool pos = int.TryParse(q.Split('&')[1], out offset);
                if (!pos)
                    return ("Please enter a valid offset.");
                q = q.Split('&')[0];
            }

            q = q.Trim();
            if (q != "")
                user = GetDiscordID(q);

            Dictionary<string, RacerData> results = GetRaces(user, offset);
            if (results.Keys.Count == 0)
                return ("No results found.");

            StringBuilder sb = new StringBuilder();
            sb.Append("Displaying necrobot results for " + GetDiscordName(user) + " \n\n");
            foreach (RacerData rd in results.Values)
            {
                sb.Append(rd.RaceData.Timestamp + "  ");
                sb.Append(rd.RaceData.Character);
                for (int i = rd.RaceData.Character.Length; i <= 10; i++) { sb.Append(" "); }
                sb.Append(rd.RaceData.Descriptor + " ");
                for (int i = rd.RaceData.Seed.Length; i <= 8; i++) { sb.Append(" "); }
                sb.Append(rd.RaceData.Seed);
                sb.Append(TimeToString(rd.Time) + "\t");
                sb.Append(RankToString(rd.Rank) + "\t");
                sb.Append(rd.Comment + "\n");
            }
            return (sb.ToString());
        }

        public static string TimeToString(string time)
        {
            TimeSpan t = TimeSpan.FromSeconds(int.Parse(time) / 100);
            return ("  " + t.ToString(@"mm\:ss"));
        }

        public static string RankToString(string rank)
        {
            switch (rank)
            {
                case ("1"):
                    return ("1st");
                case ("2"):
                    return ("2nd");
                case ("3"):
                    return ("3rd");
                default:
                    return (rank + "th");
            }
        }
    }
}
