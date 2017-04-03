using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Statsbot
{

    public enum Character { All, Aria, Bard, Bolt, Cadence, Coda, Diamond, Dorian, Dove, Eli, Melody, Monk, Nocturna, Story }
    public enum RunType { Deathless, Score, Speed }
    public enum Mode { Standard, Hardmode, NoReturn }
    public enum Product { Classic, Amplified }

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

        public static string SearchPlayers(string q)
        {
            StringBuilder sb = new StringBuilder();
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

        public static string PlayerScores(string args, RunType type)
        {

            PlayerEntries playerEntries = new PlayerEntries();

            if (args.Length < 1)
                return ("Missing name to search.");
            string name = args.Split(' ')[0];

            if (name.StartsWith("\""))
            {
                name = args.Split('\"')[0];
            }

            if (name.StartsWith("#"))
            {
                name = name.Replace("#", null);
                playerEntries = ApiSender.GetPlayerScores(name);
            }

            else
            {
                PlayerNames results = ApiSender.GetNames(name);
                if (results.Players.GetLength(0) == 0)
                    return ("Couldn't find results for \"" + name + "\".");
                else
                    playerEntries = ApiSender.GetPlayerScores(results.Players[0].ID);
            }

            Category filter = new Category();

            if (args.Contains("classic"))
                filter.Product = Product.Classic;

            if (args.Contains("noreturn"))
            {
                filter.Mode = Mode.NoReturn;
            }

            if (args.Contains("hardmode"))
            {
                filter.Mode = Mode.Hardmode;
            }

            if (args.Contains("seeded"))
            {
                filter.Seeded = true;
            }

            StringBuilder sb = new StringBuilder();

            sb.Append("Player: " + playerEntries.Player.Display_name + "\n");
            sb.Append("SteamID: " + playerEntries.Player.ID + "\n\n");
            sb.Append("Displaying " + RunToString(type, filter.Seeded) + " results " + "(" + filter.Product + ", " + filter.Mode + ")\n");


            foreach (PlayerEntry en in playerEntries.Entries)
            {
                Category cat = en.Leaderboard.ToCategory();
                if (cat.Type == type && cat.Seeded == filter.Seeded && cat.Product == filter.Product && cat.Mode == filter.Mode)
                {
                    sb.Append("\t" + cat.Char);
                    for (int i = cat.Char.ToString().Length + cat.Product.ToString().Length; i < 17; i++)
                    {
                        sb.Append(" ");
                    }
                    sb.Append(ScoreToString(en.Score, cat.Type, cat.Char, cat.Product));
                    for (int i = Digits(en.Rank); i < 6; i++) { sb.Append(" "); }
                    sb.Append(RankToString(en.Rank) + "\n");
                }
            }
            if (sb.Length < 110)
                return ("No entries found. Is this a valid category?");
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

            sb.Append("Deaths: " + stats["Deaths"] + " (" + (stats["Deaths"] / (float)(time.Playtime_forever / 60)) + " per hour)\n");
            sb.Append("Green bats killed: " + stats["Bats"] + "\n\n");
            sb.Append("Character clears ");
            foreach (string s in (Enum.GetNames(typeof(Character))))
            {
                if (stats[s] != 0)
                {
                    sb.Append("\n   " + s);
                    for (int i = s.Length + Digits(stats[s]); i < 12; i++)
                    { sb.Append(" "); }
                    sb.Append(stats[s]);
                    switch (s)
                    {
                        case "Cadence":
                            sb.Append(" (" + stats["Speedruns"] + " sub-15, " + stats["Dailies"] + " dailies)");
                            break;
                        case "Aria":
                            sb.Append(" (" + stats["AriaLow"] + " low%)");
                            break;
                        case "All":
                            sb.Append(" (" + stats["Lowest"] + " low%)");
                            break;
                    }
                }
            }
            return sb.ToString();
        }

        public static string Leaderboard(string q)
        {

            Category lb = new Category();

            string character = q.Split(new[] { ':' })[0];
            character = character.Replace(" ", null);
            for (int i = 0; i < 15; i++)
            {
                if (i == 15)
                    return ("Please enter a valid character.");
                if (character.Equals(Enum.GetNames(typeof(Character))[i], StringComparison.InvariantCultureIgnoreCase))
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
                lb.Product = Product.Classic;
                type = type.Replace("classic", null);
            }

            if (type.Contains("noreturn"))
            {
                type = type.Replace("noreturn", null);
                lb.Mode = Mode.NoReturn;
            }

            if (type.Contains("hardmode"))
            {
                type = type.Replace("hardmode", null);
                lb.Mode = Mode.Hardmode;
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

            List<SteamEntry> entries = XmlParser.ParseLeaderboard(lb, offset);
            ApiSender.GetSteamNames(entries);

            if (entries.Count == 0)
                return ("No entries found for " + lb.DisplayName + " ( offset = " + offset + " ).");

            StringBuilder sb = new StringBuilder();
            int digitR = Digits(offset + 10);

            sb.Append("Displaying results for " + lb.DisplayName + "\n\n");
            foreach (SteamEntry en in entries)
            {
                if (Digits(en.Rank) < digitR)
                    sb.Append("0");

                sb.Append(en.Rank + ".   "
                    + ScoreToString(en.Score, lb.Category.Type, lb.Category.Char, lb.Category.Product)
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

        public static string RunToString(RunType type, bool seeded)
        {
            if (seeded)
                return ("Seeded-" + type);
            else
                return (type.ToString());
        }

        public static string ScoreToString(int score, RunType type, Character character, Product product)
        {
            StringBuilder sb = new StringBuilder();
            switch (type)
            {
                case RunType.Score:
                    for (int i = Digits(score); i < 7; i++)
                    {
                        sb.Append(" ");
                    }
                    return (sb.ToString() + score.ToString());

                case RunType.Speed:

                    score = 100000000 - score;
                    TimeSpan t = TimeSpan.FromMilliseconds(score);
                    if (score >= 3600000)
                        return (t.ToString(@"h\:mm\:ss\.ff"));
                    return ("  " + t.ToString(@"mm\:ss\.ff"));

                case RunType.Deathless:
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
                    if (product == Product.Amplified)
                        return (sb.ToString() + wins + " (" + (5 - (score / 10)) + "-" + (score % 10 + 1) + ")");
                    else
                        return (sb.ToString() + wins + " (" + (4 - (score / 10)) + "-" + (score % 10 + 1) + ")");
                default:
                    return "error";
            }



        }

    }

}
