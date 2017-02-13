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

        public static int FlipZone(int i)
        {
            switch (i)
            {
                case 0:
                    i = 5;
                    break;
                case 1:
                    i = 4;
                    break;
                case 2:
                    i = 3;
                    break;
                case 3:
                    i = 2;
                    break;
                case 4:
                    i = 1;
                    break;
            }
            return (i);
        }

        public static string ScoreToString(int score, string type, string character, bool amplified)
        {
            string s = "";
            switch (type)
            {
                case "Score":
                case "SeededScore":
                    for (int i = Digits(score); i < 7; i++)
                    {
                        s += " ";
                    }
                    return (s + score.ToString());

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
                        s += " ";
                    }
                    int wins = score / 100;
                    score = score % 100;
                    if (!character.Equals("Aria"))
                        return (s + wins + " (" + (score / 10 + 1) + "-" + (score % 10 + 1) + ")");
                    if (amplified)
                        return (s + wins + " (" + FlipZone(score / 10) + "-" + (score % 10 + 1) + ")");
                    else
                        return (s + wins + " (" + FlipZone(score / 10 + 1) + "-" + (score % 10 + 1) + ")");
                default:
                    return "error";
            }

        }

        public static string ToofzCommand(string q)
        {

            if (q.Contains("​")) //gets rid of invisible space
                q = q.Replace("​", null);

            q = q.ToLower();

            string str = q.Split(new[] { ' ' })[0];


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
                    return ("ToofzBot v0.83 (now with stats!). Type \"help\" for a list of commands.");
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
            string str = "";
            bool isId = q.StartsWith("#");
            if (isId)
                q = q.Replace("#", null);

            if (!q.Contains(":") && !isId) //if the search isnt specified
            {
                PlayerResults players = ApiSender.GetPlayerResults(q);

                if (players.Players.GetLength(0) == 0)
                    return ("No results found for the name \"" + q + "\".");

                str += "Displaying top results for the name \"" + q + "\"\n\n";
                for (int i = 0; i < players.Players.GetLength(0) && i < 5; i++)
                {
                    Player player = players.Players[i];
                    str += player.Display_name + "\n";
                    str += "\tSteam ID : " + player.Id + "\n";
                }
                return str;
            }

            string name = q.Split(new[] { ':' })[0];
            string type = q.Split(new[] { ':' })[1];
            type = type.Replace(" ", null);

            return (DisplayPlayerResults(name, type));
        }

        public static PlayerEntries GetPlayer(string name)
        {
            PlayerEntries playerEntries;

            bool isId = name.StartsWith("#");
            if (isId)
                name = name.Replace("#", null);

            if (isId)
            {
                try { playerEntries = ApiSender.GetPlayerEntries(name); }
                catch { return new PlayerEntries(); }
            }

            else
            {
                PlayerResults results = ApiSender.GetPlayerResults(name);
                if (results.Players.GetLength(0) == 0)
                    return new PlayerEntries();

                playerEntries = ApiSender.GetPlayerEntries(results.Players[0].Id);
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


            string str = "";

            str += "Player: " + playerEntries.Player.Display_name + "\n";
            str += "SteamID: " + playerEntries.Player.Id + "\n";
            str += "Top " + type + " results\n";

            foreach (Entry en in playerEntries.Entries)
            {
                if (type.Equals(en.Leaderboard.Run, StringComparison.OrdinalIgnoreCase))
                {
                    str += "\t" + en.Leaderboard.Character + " (" + en.Leaderboard.Product + ")";
                    for (int i = en.Leaderboard.Character.Length + en.Leaderboard.Product.Length; i < 17; i++)
                    {
                        str += " ";
                    }
                    str += ScoreToString(en.Score, en.Leaderboard.Run, en.Leaderboard.Character, en.Leaderboard.Product.Equals("Amplified"))
                            + " | " + en.Rank + "\n";
                }
            }
            return str;
        }

        public static string SearchPlayerStats(string q)
        {
            Player player = GetPlayer(q).Player;
            if (player == null)
                return ("Couldn't find results for \"" + q + "\".");
            PlayerStats playerStats = StatsResponse.GetPlayerStats(player.Id);
            playerStats.OrganizeStats();
            Dictionary<string, int> stats = playerStats.Organized;
            Playtime time = PlaytimeResponse.GetPlaytime(player.Id);

            string str = "";
            str += "Player: " + player.Display_name + "\n";
            str += "SteamID: " + player.Id + "\n";
            str += "Total playtime: " + time.Playtime_forever / 60 + " hours (" + time.Playtime_2weeks / 60 + " recently)\n\n";

            str += "Deaths: " + stats["Deaths"] + " (" + (stats["Deaths"] / (float)(time.Playtime_forever / 60)) + " per hour)\n";
            str += "Green bats killed: " + stats["Bats"] + "\n";
            str += "Zone clears: " + stats["Z1"] + " | " + stats["Z2"] + " | " + stats["Z3"] + " | " + stats["Z4"] + "\n";
            str += "Character clears ";
            foreach (string s in PlayerStats.Characters)
            {
                if (stats[s] != 0)
                {
                    str += "\n   " + s;
                    for (int i = s.Length; i < 10; i++)
                    { str += " "; }
                    str += stats[s];
                    switch (s)
                    {
                        case "Cadence":
                            str += " (" + stats["Speedruns"] + " sub-15, " + stats["Dailies"] + " dailies)";
                            break;
                        case "Aria":
                            str += " (" + stats["AriaLow"] + " low%)";
                            break;
                        case "All":
                            str += " (" + stats["Lowest"] + " low%)";
                            break;
                    }
                }
            }
            return str;

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

            string character = q.Split(new[] { ':' })[0];
            character = character.Replace(" ", null);
            string type = q.Split(new[] { ':' })[1];
            int offset = 0;
            Leaderboard lb = null;

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

            type = type.Trim();

            try
            {
                lb = ApiSender.GetLeaderboardId(character, type, amplified).Leaderboards[0];
            }
            catch
            {
                return ("Please enter a valid leaderboard.");
            }

            LeaderboardEntries leaderboard = ApiSender.GetLeaderboardEntries(lb.Id, offset);

            if (leaderboard.Entries.Length == 0)
                return ("No entries found for " + lb.Character + " / " + lb.Run + " ( offset = " + offset + " ).");

            string str = "";
            int digitR = Digits(offset + 15);

            Entry en = null;

            str += "Displaying leaderboard results for " + lb.Display_name + "\n\n";
            for (int i = offset; i < (leaderboard.Entries.GetLength(0)) && i < offset + 15; i++)
            {
                en = leaderboard.Entries[i - offset];

                if (Digits(en.Rank) < digitR)
                    str += "0";

                str += en.Rank + ".   "
                    + ScoreToString(en.Score, lb.Run, lb.Character, amplified)
                    + "\t" + en.Player.Display_name + "\n";
            }
            return str;

        }

    }

}
