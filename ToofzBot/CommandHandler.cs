using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.Text.RegularExpressions;

namespace ToofzBot
{

    public static class CommandHandler
    {

        public static int Digits(int i)
        {
            if (i == 0)
                return 1;
            return (int)Math.Floor(Math.Log10(i) + 1);
        }


        public static string ScoreToString(int score, string type, string character, bool amplified)
        {
            StringBuilder sb = new StringBuilder();
            switch (type)
            {
                case "Score":
                case "SeededScore":
                    for (int i = Digits(score); i < 7; i++)
                    {
                        sb.Append(" ");
                    }
                    return (sb.ToString() + score.ToString());

                case "Speed":
                case "SeededSpeed":

                    score = 100000000 - score;
                    TimeSpan t = TimeSpan.FromMilliseconds(score);
                    if (score >= 3600000)
                        return (t.ToString(@"h\:mm\:ss\.ff"));
                    return ("  " + t.ToString(@"mm\:ss\.ff"));

                case "Deathless":
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
                        return (sb.ToString() + wins + " (" + (4 - (score / 10 + 1)) + "-" + (score % 10 + 1) + ")");
                default:
                    return "error";
            }

        }

        public static string ToofzCommand(string q)
        {

            if (q.Contains("​")) //gets rid of invisible character
                q = q.Replace("​", null);

            q = q.ToLower();

            string str = q.Split(' ')[0];


            switch (str)
            {
                case "search":
                    q = q.Replace("search ", null);
                    return (SearchName(q));
                case "leaderboard":
                    q = q.Replace("leaderboard ", null);
                    return (SearchLeaderboard(q));
                case "help":
                    q = q.Replace("help ", null);
                    return HelpCommand(q);
                case "info":
                    return ("ToofzBot v0.84. Type \"help\" for a list of commands.");
                default:
                    return ("Unknown command. Use \"search\", \"leaderboard\", or \"help <command>\" for more information.");
            }
        }

        public static string HelpCommand(string q)
        {
            if (q.StartsWith("search"))
                return ("Searches for a player."
                    + "\nType \".toofz search <name>\" to see a list of search results, or \".toofz search <name>: <category>\" to see results for a specific category."
                    + "\nSteamID can be used instead of a name (\".toofz search #245356: seeded score\").");
            if (q.StartsWith("leaderboard"))
                return ("Displays a leaderboard."
                    + "\nType \".toofz leaderboard <character>: <category>\" to see a leaderboard."
                    + "\nAdd \"classic\" before the character name to see results without Amplified."
                    + "\nAdd \"&<rank>\" or \"offset=<rank>\" to see the result starting at the specified offset.");
            return ("ToofzBot is a bot which retrieves Crypt of the Necrodancer player stats."
                + "\n\tAvailable commands: \"search\", \"leaderboard\", \"info\"."
                + "\nUse \"search\", \"leaderboard\", or \"help <command>\" for more information."
                + "\nPing Naymin#5067 for questions and bug reports.");
        }

        public static string SearchName(string q)
        {
            StringBuilder sb = new StringBuilder();
            bool isID = q.StartsWith("#");
            if (isID)
                q = q.Replace("#", null);

            if (!q.Contains(":") && !isID) //if the search isnt specified
            {
                PlayerResults players = ApiSender.GetNames(q);

                if (players.Players.GetLength(0) == 0)
                    return ("No results found for \"" + q + "\".");

                sb.Append("Displaying top results for \"" + q + "\"\n\n");
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

            return (DisplayPlayerResults(name, type));
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
                PlayerResults results = ApiSender.GetNames(name);
                if (results.Players.GetLength(0) == 0)
                    return new PlayerEntries();

                playerEntries = ApiSender.GetPlayerScores(results.Players[0].ID);
            }
            return playerEntries;
        }

        public static string DisplayPlayerResults(string name, string type)
        {

            PlayerEntries playerEntries = GetPlayer(name);
            if (playerEntries == null)
                return ("Couldn't find results for \"" + name + "\".");
            if (playerEntries.Entries.Length == 0)
                return ("No entries found for " + name + " in the category " + type + ".");

            StringBuilder sb = new StringBuilder();

            sb.Append("Player: " + playerEntries.Player.Display_name + "\n");
            sb.Append("SteamID: " + playerEntries.Player.ID + "\n\n");
            sb.Append("Top " + type + " results\n");

            foreach (Entry en in playerEntries.Entries)
            {
                if (type.Equals(en.Leaderboard.Run, StringComparison.OrdinalIgnoreCase))
                {
                    sb.Append("\t" + en.Leaderboard.Character + " (" + en.Leaderboard.Product + ")");
                    for (int i = en.Leaderboard.Character.Length + en.Leaderboard.Product.Length; i < 17; i++)
                    {
                        sb.Append(" ");
                    }
                    sb.Append(ScoreToString(en.Score, en.Leaderboard.Run, en.Leaderboard.Character, en.Leaderboard.Product.Equals("Amplified"))
                            + " | " + en.Rank + "\n");
                }
            }
            return sb.ToString();
        }

        public static string SearchPlayerStats(string q)
        {
            Player player = GetPlayer(q).Player;
            if (player == null)
                return ("Couldn't find results for \"" + q + "\".");
            PlayerStats playerStats = ApiSender.GetPlayerStats(player.ID);
            if (playerStats.Stats == null)
                return ("Couldn't access " + player.Display_name + "'s profile (likely private).");
            playerStats.OrganizeStats();
            Dictionary<string, int> stats = playerStats.Organized;
            Playtime time = PlaytimeResponse.GetPlaytime(player.ID);

            StringBuilder sb = new StringBuilder();
            sb.Append("Player: " + player.Display_name + "\n");
            sb.Append("SteamID: " + player.ID + "\n");
            sb.Append("Total playtime: " + time.Playtime_forever / 60 + " hours (" + time.Playtime_2weeks / 60 + " recently)\n\n");

            sb.Append("Deaths: " + stats["deaths"] + " (" + (stats["deaths"] / (float)(time.Playtime_forever / 60)) + " per hour)\n");
            sb.Append("Green bats killed: " + stats["bats"] + "\n");
            sb.Append("Zone clears: " + stats["z1"] + " | " + stats["z2"] + " | " + stats["z3"] + " | " + stats["z4"] + "\n");
            sb.Append("Character clears ");
            foreach (string s in PlayerStats.Characters)
            {
                if (stats[s] != 0)
                {
                    sb.Append("\n   " + s);
                    for (int i = s.Length; i < 10; i++)
                    { sb.Append(" "); }
                    sb.Append(stats[s]);
                    switch (s)
                    {
                        case "Cadence":
                            sb.Append(" (" + stats["speedruns"] + " sub-15, " + stats["dailies"] + " dailies)");
                            break;
                        case "Aria":
                            sb.Append(" (" + stats["ariaLow"] + " low%)");
                            break;
                        case "All":
                            sb.Append(" (" + stats["lowest"] + " low%)");
                            break;
                    }
                }
            }
            return sb.ToString();

        }

        public static string SearchLeaderboard(string q)
        {

            if (!q.Contains(":"))
                return ("Please enter a specific leaderboard in the form of \"-leaderboard <character>: <type> &<offset>\".");

            bool amplified = true;
            if (q.StartsWith("classic"))
            {
                amplified = false;
                q = q.Replace("classic", null);
            }

            string[] split = q.Split(':');
            string character = split[0];
            character = character.Replace(" ", null);
            string type = split[1];
            int offset = 0;
            Leaderboard lb = null;

            if (type.Contains("offset"))
            {
                type = type.Replace("offset", "&");
                type = type.Replace("=", null);
            }

            if (type.Contains("&"))
            {
                bool pos = int.TryParse(type.Split('&')[1], out offset);
                if (!pos)
                    return ("Please enter a valid offset.");
                type = type.Split('&')[0];
            }

            type = type.Replace(" ", null);

            try
            {
                lb = ApiSender.GetLeaderboardID(character, type, amplified).Leaderboards[0];
            }
            catch
            {
                return ("Please enter a valid leaderboard.");
            }

            LeaderboardEntries leaderboard = ApiSender.GetLeaderboardEntries(lb.ID, offset);

            if (leaderboard.Entries.Length == 0)
                return ("No entries found for " + lb.Character + " / " + lb.Run + " ( offset = " + offset + " ).");

            StringBuilder sb = new StringBuilder();
            int digitR = Digits(offset + 15);

            Entry en = null;

            sb.Append("Displaying leaderboard results for " + lb.Display_name + "\n\n");
            for (int i = offset; i < (leaderboard.Entries.GetLength(0)) && i < offset + 15; i++)
            {
                en = leaderboard.Entries[i - offset];

                if (Digits(en.Rank) < digitR)
                    sb.Append("0");

                sb.Append(en.Rank + ".   "
                    + ScoreToString(en.Score, lb.Run, lb.Character, amplified)
                    + "\t" + en.Player.Display_name + "\n");
            }
            return sb.ToString();

        }

    }

}
