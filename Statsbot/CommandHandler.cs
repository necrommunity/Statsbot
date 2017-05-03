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
            if (MultiContains(q, new[] { "search", "s" }))
                return ("Searches for players and their steam IDs."
                    + "\nUse \".search <name>\" to see a list of search results."
                    + "\nUse \".speed, .score, or .deathless <name>\" to see the player's results in a specific category.");
            if (MultiContains(q, new[] { "speed", "score", "deathless" }))
                return ("Displays specific player's personal bests in a specific category."
                    + "\nUse \".speed, .score, or .deathless <name>\" to see the player's results in that category."
                    + "\nAdd \"seeded\" to see the seeded leaderboards, \"classic\" to see results without the dlc, and \"hardmode\" or \"noreturn\" for the extra play modes."
                    + "\nSteamID can be used instead of a name (\".speed #76561198000263514\").");
            if (MultiContains(q, new[] { "leaderboard", "lb" }))
                return ("Displays a leaderboard."
                    + "\nUse \".leaderboard <character> <category>\" to see a leaderboard. Speed is the default in case no category is entered."
                    + "\nAdd \"seeded\" to see the seeded leaderboards, \"classic\" to see results without the dlc, and \"hardmode\" or \"noreturn\" for the extra play modes."
                    + "\nAdd \"&<offset>\" in the end to see the result starting at the specified offset.");
            if (MultiContains(q, new[] { "records", "stats" }))
                return ("Displays a steam user's stats."
                    + "\nType \".records <profile name> to display said user's recorded stats.");
            if (MultiContains(q, new[] { "necrobot", "race" }))
                return ("Displays a user's recorded necrobot races."
                    + "\nType \".necrobot <user>\" to see past races."
                    + "\nAdd \"&<offset>\" in the end to see older results.");
            if (q.StartsWith("version"))
                return ("Displays the bot's current version.");
            return ("Statsbot is a bot which retrieves Crypt of the Necrodancer player stats."
                + "\n\nAvailable commands: search(s), speed, score, deathless, leaderboard(lb), records(stats), necrobot(races), version."
                + "\nUse \".help <command>\" for more information."
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

            string name = args;

            if (args.Contains(' '))
                name = args.Split(' ')[0];

            if (name.StartsWith("\""))
                name = args.Split('\"')[0];


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

            if (MultiContains(args, new[] { "classic", "base" }))
                filter.Product = Product.Classic;

            if (MultiContains(args, new[] { "return", "nr" }))
                filter.Mode = Mode.NoReturn;

            if (args.Contains("hard"))
                filter.Mode = Mode.Hardmode;

            if (args.Contains("seeded"))
                filter.Seeded = true;

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

            for (int i = 0; i < 15; i++)
            {
                if (i == 14)
                    return ("Please enter a valid character.");
                if (InvariantContains(q, Enum.GetNames(typeof(Character))[i]))
                {
                    lb.Char = (Character)i;
                    q.Replace(lb.Char.ToString(), null);
                    break;
                }
            }

            for (int i = 0; i < 3; i++)
            {
                if (InvariantContains(q, Enum.GetNames(typeof(RunType))[i]))
                {
                    lb.Type = (RunType)i;
                    q.Replace(lb.Type.ToString(), null);
                    break;
                }
            }

            int offset = 1;

            if (q.Contains("&"))
            {
                bool pos = int.TryParse(q.Split(new[] { '&' })[1], out offset);
                if (!pos)
                    return ("Please enter a valid offset.");
                q = q.Split(new[] { '&' })[0];
            }

            if (MultiContains(q, new[] { "classic", "base" }))
                lb.Product = Product.Classic;

            if (MultiContains(q, new[] { "return", "nr" }))
                lb.Mode = Mode.NoReturn;

            if (q.Contains("hard"))
                lb.Mode = Mode.Hardmode;

            if (q.Contains("seeded"))
                lb.Seeded = true;

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
                    for (; d < 7; d++)
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

        public static bool InvariantContains(string source, string check)
        {
            return (source.IndexOf(check, StringComparison.InvariantCultureIgnoreCase)) >= 0;
        }

        public static bool MultiContains(string source, string[] check)
        {
            foreach (string s in check)
                if (InvariantContains(source, s))
                    return true;
            return false;
        }

    }

}
