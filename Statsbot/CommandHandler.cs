using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Statsbot
{

    public static class CommandHandler
    {
        public static string Help(string q)
        {
            if (q.StartsWith("search") || q.StartsWith("player") || q.StartsWith("toofz"))
                return ("Searches for players and their top scores."
                    + "\nType \".statsbot search <name>\" to see a list of search results, or \".statsbot search <name>: <category>\" to see results for a specific category."
                    + "\nSteamID can be used instead of a name (\".statsbot search #245356: seeded score\").");
            if (q.StartsWith("leaderboard") || q.StartsWith("lb"))
                return ("Displays a leaderboard."
                    + "\nType \".statsbot leaderboard <character>: <category>\" to see a leaderboard."
                    + "\nAdd \"classic\" before the category to see results without the dlc."
                    + "\nAdd \"&<offset>\" after the category to see the result starting at the specified offset.");
            if (q.StartsWith("record") || q.StartsWith("stats"))
                return ("Displays a steam user's stats."
                    + "\nType \".stats <profile name> to display said user's stats.");
            if (q.StartsWith("necrobot") || q.StartsWith("races"))
                return ("Displays a user's recorded necrobot races."
                    + "\nType \".statsbot necrobot <user>\" to see past races."
                    + "\nAdd \"&<offset>\" to see older results.");
            if (q.StartsWith("version"))
                return ("Displays the bot's current version.");
            return ("Statsbot is a bot which retrieves Crypt of the Necrodancer player stats."
                + "\n\nAvailable commands: \"search/player/toofz\", \"leaderboard/lb\", \"records/stats\", \"necrobot/races\", \"version\"."
                + "\nUse \".statsbot help <command>\" to expand on a command."
                + "\n\nFull documentation is available at https://github.com/necrommunity/Statsbot"
                + "\nContact Naymin#5067 for feedback and bug reports."
                );
        }

        public static string Search(string q)
        {
            StringBuilder sb = new StringBuilder();
            if (!q.Contains(":")) //if the search isnt specified
            {
                PlayerNames players = ApiSender.GetNames(q);

                if (players.Players.Length == 0)
                    return ("No results found for \"" + q + "\".");

                sb.Append("Displaying top players for \"" + q + "\"\n\n");
                for (int i = 0; i < players.Players.GetLength(0) && i < 5; i++)
                {
                    Player player = players.Players[i];
                    sb.Append(player.Display_name + "\n");
                    sb.Append("\tSteam ID : " + player.ID + "\n");
                }
                return sb.ToString();
            }

            string[] split = q.Split(':');
            string name = split[0];
            string type = split[1];
            type = type.Replace(" ", null);

            return (PlayerScores(name, type));
        }

        public static PlayerEntries GetPlayer(string name)
        {
            PlayerEntries playerEntries;

            bool isID = name.StartsWith("#");
            if (isID)
                name = name.Replace("#", null);

            if (isID)
            {
                try { playerEntries = ApiSender.GetPlayerScores(name); }
                catch { return new PlayerEntries(); }
            }

            else
            {
                PlayerNames results = ApiSender.GetNames(name);
                if (results.Players.GetLength(0) == 0)
                    return new PlayerEntries();

                playerEntries = ApiSender.GetPlayerScores(results.Players[0].ID);
            }
            return playerEntries;
        }

        public static string PlayerScores(string name, string type)
        {

            PlayerEntries playerEntries = new PlayerEntries();

            bool id = name.StartsWith("#");
            if (id)
            {
                name = name.Replace("#", null);
                playerEntries = ApiSender.GetPlayerScores(name);
            }

            else
            {
                PlayerNames results = ApiSender.GetNames(name);
                if (!(results.Players.GetLength(0) == 0))
                    playerEntries = ApiSender.GetPlayerScores(results.Players[0].ID);
            }

            if (playerEntries.Player == null || playerEntries.Entries.Length == 0)
                return ("Couldn't find results for \"" + name + "\".");

            string product = "amplified";

            if (type.Contains("amplified"))
                type = type.Replace("amplified", null);

            if (type.Contains("classic"))
            {
                product = "classic";
                type = type.Replace("classic", null);
                type = type.Trim();
            }

            string mode = "standard";
            if (type.Contains("return"))
            {
                type = type.Replace("no return", null);
                type = type.Replace("noreturn", null);
                type = type.Replace("return", null);
                mode = "no-return";
            }

            if (type.Contains("hard"))
            {
                type = type.Replace("hardmode", null);
                type = type.Replace("hard mode", null);
                type = type.Replace("hard", null);
                mode = "hard-mode";
            }

            string seeded = "";
            if(type.Contains("seeded"))
            {
                type = type.Replace("seeded", null);
                seeded = "seeded-";
            }

            StringBuilder sb = new StringBuilder();

            sb.Append("Player: " + playerEntries.Player.Display_name + "\n");
            sb.Append("SteamID: " + playerEntries.Player.ID + "\n\n");
            sb.Append("Top " + seeded + type + " results " + "(" + product + ", " + mode + ")\n");

            foreach (PlayerEntry en in playerEntries.Entries)
            {
                if (en.Leaderboard.Run == seeded + type && en.Leaderboard.Product == product && en.Leaderboard.Mode == mode)
                {
                    sb.Append("\t" + en.Leaderboard.Character);
                    for (int i = en.Leaderboard.Character.Length + en.Leaderboard.Product.Length; i < 17; i++)
                    {
                        sb.Append(" ");
                    }
                    sb.Append(ScoreToString(en.Score, en.Leaderboard.Run, en.Leaderboard.Character, en.Leaderboard.Product.Equals("amplified")));
                    for (int i = Digits(en.Rank); i < 6; i++) { sb.Append(" "); }
                    sb.Append(RankToString(en.Rank) + "\n");
                }
            }
            return sb.ToString();
        }

        public static string Records(string q)
        {
            Player player = GetPlayer(q).Player;
            if (player == null)
                return ("Couldn't find results for \"" + q + "\".");
            Records records = ApiSender.GetPlayerRecords(player.ID);
            if (records.Stats == null)
                return ("Couldn't access " + player.Display_name + "'s profile (likely private).");
            records.Organize();
            Dictionary<string, int> stats = records.Organized;
            Playtime time = PlaytimeResponse.GetPlaytime(player.ID);

            StringBuilder sb = new StringBuilder();
            sb.Append("Player: " + player.Display_name + "\n");
            sb.Append("SteamID: " + player.ID + "\n");
            sb.Append("Total playtime: " + time.Playtime_forever / 60 + " hours (" + time.Playtime_2weeks / 60 + " recently)\n\n");

            sb.Append("Deaths: " + stats["deaths"] + " (" + (stats["deaths"] / (float)(time.Playtime_forever / 60)) + " per hour)\n");
            sb.Append("Green bats killed: " + stats["bats"] + "\n");
            sb.Append("Zone clears: " + stats["z1"] + " | " + stats["z2"] + " | " + stats["z3"] + " | " + stats["z4"] + "\n");
            sb.Append("Character clears ");
            bool nl = true;
            foreach (string s in (Enum.GetNames(typeof(Character))))
            {
                if (stats[s] != 0)
                {
                    if (nl)
                        sb.Append("\n   " + s);
                    else
                        sb.Append("\t" + s);
                    for (int i = s.Length + Digits(stats[s]); i < 14; i++)
                    { sb.Append(" "); }
                    sb.Append(stats[s]);
                    switch (s)
                    {
                        case "Cadence":
                            sb.Append(" -  " + stats["speedruns"] + " sub-15, " + stats["dailies"] + " dailies");
                            nl = false;
                            break;
                        case "Aria":
                            sb.Append(" -  " + stats["ariaLow"] + " low%");
                            nl = false;
                            break;
                        case "All":
                            sb.Append(" -  " + stats["lowest"] + " low%");
                            nl = false;
                            break;
                    }
                    if (nl == true)
                        nl = false;
                    else
                        nl = true;
                }
            }
            return sb.ToString();
        }

        public static string Leaderboard(string q)
        {

            if (!q.Contains(":"))
                q = q + ":speed";

            Category lb = new Category();

            string character = q.Split(new[] { ':' })[0];
            character = character.Replace(" ", null);
            for (int i = 0; i < 14; i++)
            {
                if (i == 14)
                    return ("Please enter a valid character.");
                if (character.Contains(Enum.GetNames(typeof(Character))[i].ToLower()))
                {
                    lb.Char = (Character)i;
                    break;
                }
            }

            string type = q.Split(new[] { ':' })[1];

            int offset = 1;
            if (type.Contains("offset"))
            {
                type = type.Replace("offset", "&");
                type = type.Replace("=", null);
            }

            if (type.Contains("&"))
            {
                bool pos = int.TryParse(type.Split(new[] { '&' })[1], out offset);
                if (!pos)
                    return ("Please enter a valid offset.");
                type = type.Split(new[] { '&' })[0];
            }

            if (type.Contains("classic"))
            {
                lb.Amplified = false;
                type = type.Replace("classic", null);
            }

            if (type.Contains("amplified"))
            {
                type = type.Replace("amplified", null);
            }

            if (type.Contains("return"))
            {
                type = type.Replace("no return", null);
                type = type.Replace("noreturn", null);
                type = type.Replace("return", null);
                lb.Mode = Mode.NoReturn;
            }

            if (type.Contains("hard"))
            {
                type = type.Replace("hardmode", null);
                type = type.Replace("hard mode", null);
                type = type.Replace("hard", null);
                lb.Mode = Mode.Hard;
            }

            if (type.Contains("seeded"))
            {
                lb.Seeded = true;
                type = type.Replace("seeded", null);
            }


            type = type.Trim();

            for (int i = 0; i < 4; i++)
            {
                if (i == 3)
                    return ("Please enter a valid category.");
                if (type.Contains(Enum.GetNames(typeof(RunType))[i].ToLower()))
                {
                    lb.Type = (RunType)i;
                    break;
                }
            }
            return LeaderboardString(lb, offset);
        }

        public static string LeaderboardString(Category category, int offset)
        {
            Leaderboard lb = new Leaderboard();

            try { lb = XmlParser.lbInfo[category]; }
            catch { return ("Please enter a valid leaderboard."); }

            List<Entry> entries = XmlParser.ParseLeaderboard(lb, offset);
            ApiSender.GetSteamNames(entries);

            if (entries.Count == 0)
                return ("No entries found for " + lb.DisplayName + " ( offset = " + offset + " ).");

            StringBuilder sb = new StringBuilder();
            int digitR = Digits(offset + 10);

            sb.Append("Displaying results for " + lb.DisplayName + "\n\n");
            foreach (Entry en in entries)
            {
                if (Digits(en.Rank) < digitR)
                    sb.Append("0");

                sb.Append(en.Rank + ".   "
                    + ScoreToString(en.Score, lb.Category.GetRunString(), lb.Category.Char.ToString(), lb.Category.Amplified)
                    + "\t" + en.Personaname + "\n");
            }
            return sb.ToString();
        }

        public static int Digits(int i)
        {
            if (i == 0)
                return 1;
            return (int)Math.Floor(Math.Log10(i) + 1);
        }

        public static string RankToString(int rank)
        {
            if ((rank % 100) / 10 == 1)
            {
                return (rank + "th");
            }

            switch (rank % 10)
            {
                case (1):
                    return (rank + "st");
                case (2):
                    return (rank + "nd");
                case (3):
                    return (rank + "rd");
                default:
                    return (rank + "th");
            }
        }

        public static string ScoreToString(int score, string type, string character, bool amplified)
        {
            StringBuilder sb = new StringBuilder();
            switch (type)
            {
                case "Score":
                case "SeededScore":
                case "score":
                case "seeded-score":
                    for (int i = Digits(score); i < 7; i++)
                    {
                        sb.Append(" ");
                    }
                    return (sb.ToString() + score.ToString());

                case "Speed":
                case "SeededSpeed":
                case "speed":
                case "seeded-speed":

                    score = 100000000 - score;
                    TimeSpan t = TimeSpan.FromMilliseconds(score);
                    if (score >= 3600000)
                        return (t.ToString(@"h\:mm\:ss\.ff"));
                    return ("  " + t.ToString(@"mm\:ss\.ff"));

                case "Deathless":
                case "deathless":
                    int d = Digits(score);
                    if (d < 3)
                        d = 3;
                    for (; d < 5; d++)
                    {
                        sb.Append(" ");
                    }
                    int wins = score / 100;
                    score = score % 100;
                    if (!character.Equals("Aria"))
                        return (sb.ToString() + wins + " (" + (score / 10 + 1) + "-" + (score % 10 + 1) + ")");
                    if (amplified)
                        return (sb.ToString() + wins + " (" + (5 - (score / 10)) + "-" + (score % 10 + 1) + ")");
                    else
                        return (sb.ToString() + wins + " (" + (4 - (score / 10)) + "-" + (score % 10 + 1) + ")");
                default:
                    return "error";
            }



        }

    }

}
