using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Globalization;

namespace Statsbot
{
    public class Racebot
    {

        private MySqlConnection Connection = new MySqlConnection(Program.database.ToString());

        public string GetNecrobot(string user, int offset)
        {
            user = Filter(user);
            StringBuilder sb = new StringBuilder();
            using (Connection)
            {
                try
                {
                    Connection.Open();
                    Connection.ChangeDatabase("necrobot");
                    MySqlCommand comm = new MySqlCommand("SELECT `timestamp`, `character`, descriptor, seeded, level, time, igt, rank, comment" +
                        " FROM users, races, race_runs, race_types WHERE users.discord_name='" + Filter(user) +
                        "' AND users.user_id=race_runs.user_id AND races.type_id=race_types.type_id" +
                        " AND races.race_id=race_runs.race_id ORDER BY races.race_id DESC", Connection);
                    using (MySqlDataReader r = comm.ExecuteReader())
                    {
                        for (int i = 0; i < (offset + 10) && r.Read(); i++) //while havn't reached end of reader
                        {
                            if (i >= offset) //starts reading at specified offset
                            {
                                sb.Append(r.GetDateTime(0).ToString("yyyy-MM-dd HH:mm") + " ");
                                sb.Append(r[1] + " ");
                                sb.Append(r[2] + " ");
                                if (r.GetBoolean(3))
                                    sb.Append("Seeded --- ");
                                else
                                    sb.Append("Unseeded --- ");
                                int level = r.GetInt16(4);
                                if (level == -2)
                                {
                                    sb.Append(CommandHandler.RankToString(r.GetInt16(7)) + ", " + TimeToString(r.GetInt32(5)));
                                    if (r.GetInt32(6) != -1)
                                        sb.Append(" (igt " + TimeToString(r.GetInt32(6)) + ")");
                                }
                                else
                                {
                                    sb.Append("Forfeit! (rta " + TimeToString(r.GetInt32(5)));
                                    if (level == 0)
                                        sb.Append(")");
                                    else if (level == 20)
                                        sb.Append(", 5-5)");
                                    else
                                        sb.Append(", " + (level / 4 + 1) + "-" + (level % 4 + 1) + ")");
                                }
                                if ((r.GetString(8)) != "")
                                    sb.Append(": " + r[8]);
                                sb.Append("\n");
                            }
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine(ex.Message);
                    return ("");
                }
            }
            return sb.ToString();
        }

        public string GetS6(string user, int offset)
        {
            user = Filter(user);
            StringBuilder sb = new StringBuilder();
            using (Connection)
            {
                try
                {
                    Connection.Open();
                    Connection.ChangeDatabase("season_6");
                    MySqlCommand comm = new MySqlCommand("SELECT timestamp, level, time, rank" +
                        " FROM necrobot.users, race_runs, races " +
                        " WHERE users.discord_name ='" + Filter(user) +
                        "' AND race_runs.user_id = necrobot.users.user_id" +
                        " AND race_runs.race_id = races.race_id" +
                        " ORDER BY race_runs.race_id DESC", Connection);
                    using (MySqlDataReader r = comm.ExecuteReader())
                    {
                        for (int i = 0; i < (offset + 10) && r.Read(); i++) //while havn't reached end of reader
                        {
                            if (i >= offset) //starts reading at specified offset
                            {
                                sb.Append(r.GetDateTime(0).ToString("yyyy-MM-dd HH:mm") + " --- ");
                                int level = r.GetInt16(1);
                                if (r.GetInt16(3) == 1)
                                    sb.Append("WIN ");
                                else
                                    sb.Append("LOSE");
                                sb.Append(" (rta " + TimeToString(r.GetInt32(2)) + ")\n");
                            }
                        }
                    }

                }
                catch (MySqlException ex)
                {
                    Console.WriteLine(ex.Message);
                    return ("");
                }
            }
            return sb.ToString();
        }

        public string Filter(string q)
        {
            string[] filter = { "select", "update", "delete", "insert", "create", "alter", "drop" };
            foreach (string s in filter)
            {
                if (q.Contains(s))
                    q = q.Replace(s, null);
            }
            return q;
        }

        public string DisplayNecrobot(string q, Discord.User user)
        {

            int offset = 0;

            if (q.Contains("&"))
            {
                bool pos = int.TryParse(q.Split('&')[1], out offset);
                if (!pos)
                    return ("Please enter a valid offset.");
                q = q.Split('&')[0];
            }

            if (q == "")
                q = user.Name;

            string results = GetNecrobot(q, offset);
            if (results == "")
                return ("No necrobot results found for \"" + q + "\".");

            StringBuilder sb = new StringBuilder();
            sb.Append("Displaying requested necrobot results for " + q + " \n\n");
            sb.Append(results);
            return (sb.ToString());
        }

        public string DisplayS6(string q, Discord.User user)
        {

            int offset = 0;

            if (q.Contains("&"))
            {
                bool pos = int.TryParse(q.Split('&')[1], out offset);
                if (!pos)
                    return ("Please enter a valid offset.");
                q = q.Split('&')[0];
            }

            if (q == "")
                q = user.Name;

            string results = GetS6(q, offset);
            if (results == "")
                return ("No S6 results found for \"" + q + "\".");

            StringBuilder sb = new StringBuilder();
            sb.Append("Displaying requested S6 results for " + q + " \n\n");
            sb.Append(results);
            return (sb.ToString());
        }


        public static string TimeToString(int time)
        {
            TimeSpan t = TimeSpan.FromSeconds(time / 100);
            return (t.ToString(@"mm\:ss"));
        }
    }
}
