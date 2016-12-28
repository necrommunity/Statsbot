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
            return (int)Math.Floor(Math.Log10(i) + 1);
        }

        public static int FlipZone(int i)
        {
            switch (i)
            {
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

        public static string ScoreToString(int score, string type, string character)
        {
            string s = "";
            switch (type)
            {
                case "score":
                case "seeded score":
                    for (int i = Digits(score); i < 7; i++)
                    {
                        s += " ";
                    }
                    return (s + score.ToString());

                case "speed":
                case "seeded speed":

                    score = 100000000 - score;
                    TimeSpan t = TimeSpan.FromMilliseconds(score);
                    if (score >= 3600000)
                        return (t.ToString(@"h\:mm\:ss\.ff"));
                    return ("  " + t.ToString(@"mm\:ss\.ff"));

                case "deathless":
                    int d = Digits(score);
                    if (d < 4)
                        d = 4;
                    for (; d < 5; d++)
                    {
                        s += " ";
                    }
                    int wins = score / 100;
                    score = score % 100;
                    if (!character.Equals("Aria"))
                        return (s + wins + " (" + (score / 10 + 1) + "-" + (score % 10 + 1) + ")");
                    else
                        return (s + wins + " (" + FlipZone(score / 10 + 1) + "-" + (score % 10 + 1) + ")");

                default:
                    return "error";
            }

        }

        public static string ToofzCommand(string q)
        {
            string str = q.Split(new[] { ' ' })[0];

            if (q.Contains("penguin"))
                return ("@ᕕ(' >')ᕗᕕ(' >')ᕗᕕ(' >')ᕗ" + "\npls no bulli");

            if (q.Contains("​")) //gets rid of invisible space
                q = q.Replace("​", null);

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
                    if (q.StartsWith("search"))
                        return ("Searches for a player."
                            + "\nType \".toofz search <name>\" to see a list of search results, or \".toofz search <name>: <category>\" to see results for a specific category."
                            + "\nSteamID can be used instead of a name (\".toofz search #245356: seeded score\").");
                    if (q.StartsWith("leaderboard"))
                        return ("Displays a leaderboard."
                            + "\nType \".toofz leaderboard <character>: <category>\" to see a leaderboard."
                            + "\nAdd \"&<rank>\" or \"offset=<rank>\" to see the result starting at the specified offset.");
                    return ("ToofzBot is a bot which retrieves Crypt of the Necrodancer player stats."
                        + "\n\tAvailable commands: \"search\", \"leaderboard\"."
                        + "\nUse \"search\", \"leaderboard\", or \"help <command>\" for more information."
                        + "\n Ping Naymin#5067 for questions and bug reports.");
                default:
                    return ("Unknown command. Use \"search\", \"leaderboard\", or \"help <command>\" for more information.");
            }
        }

        public static string SearchName(string q)
        {
            string str = "";
            bool isID = q.StartsWith("#");
            if (isID)
                q = q.Replace("#", null);

            if (!q.Contains(":")) //if the search isnt specified
            {
                PlayerResults players = ApiSender.GetPlayers(q);

                if (players.Entries.GetLength(0) == 0)
                    return ("No results found for the name \"" + q + "\".");

                str += "Displaying top results for the name \"" + q + "\"\n\n";
                for (int i = 0; i < players.Entries.GetLength(0) && i < 5; i++)
                {
                    PlayerResult player = players.Entries[i];
                    str += player.Name + "\n";
                    str += "\tSteam Id : " + player.SteamId + "\n";
                    str += "\tEntry count: " + player.EntryCount + "\n";
                    str += "\tBest entry: " + player.BestEntry.Title + " / " + player.BestEntry.Rank + "\n";
                }
                return str;
            }

            else
            {
                string name = q.Split(new[] { ':' })[0];
                string type = q.Split(new[] { ':' })[1];
                type = type.Trim();

                return (SearchPlayer(name, type, isID));
            }
        }

        public static string SearchPlayer(string name, string type, bool isID)
        {

            Player idPlayer;
            PlayerResult player;

            string str = "";

            if (isID)
            {
                SteamUser user = ApiSender.GetSteamUser(name);
                if (user.Steamid == "error")
                    return ("Couldn't find results for the steamID \"" + name + "\".");

                idPlayer = ApiSender.GetPlayersId(name.ToString());

                str += "Player: " + user.Personaname + "\n";
                str += "PlayerID: " + name + "\n";
            }

            else
            {
                PlayerResults results = ApiSender.GetPlayers(name);
                if (results.Entries.GetLength(0) == 0)
                    return ("Couldn't find results for the name \"" + name + "\".");

                player = results.Entries[0];
                idPlayer = ApiSender.GetPlayersId(player.SteamId);

                str += "Player: " + player.Name + "\n";
                str += "PlayerID: " + player.SteamId + "\n";
            }

            foreach (PlayerRun pr in idPlayer.Runs)
            {
                if (type.Equals(pr.Run.DisplayName, StringComparison.OrdinalIgnoreCase))
                {
                    str += "Top " + pr.Run.DisplayName + " results\n";
                    foreach (PlayerEntry e in pr.Entries)
                    {
                        str += "\t" + e.Character;
                        for (int i = e.Character.Length; i < 7; i++)
                        {
                            str += " ";
                        }
                        str += "\t" + ScoreToString(e.Score, pr.Run.Kind, e.Character)
                                + " | " + e.Rank + "\n";
                    }
                    return str;
                }
            }
            if (isID)
                return ("No entries found for steamID " + name + " in the category " + type + ".");
            else
                return ("No entries found for the player \"" + name + "\" in the category " + type + ".");
        }

        public static string SearchLeaderboard(string q)
        {

            if (!q.Contains(":"))
                return ("Please enter a specific leaderboard in the form of \"-leaderboard <character>: <type> &<offset>\".");

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

            Leaderboard[] boards = ApiSender.GetLeaderboards();
            foreach (Leaderboard b in boards) //looks for the board ID
            {
                if (character.Equals(b.Character, StringComparison.OrdinalIgnoreCase) && type.Equals(b.Run, StringComparison.OrdinalIgnoreCase))
                {
                    lb = b;
                    break;
                }
            }

            if (lb == null)
                return ("Please enter a valid leaderboard.");

            LeaderBoardEntries leaderboard = ApiSender.GetLeaderboardEntries(lb.LeaderboardId, offset);

            if (leaderboard.Entries.Length == 0)
                return ("No entries found for " + lb.Character + " / " + lb.Run + " ( offset = " + offset + " ).");

            string str = "";
            int digitR = Digits(offset + 15);

            Entry en = null;

            str += "Displaying leaderboard results for " + lb.Character + " / " + lb.Run + "\n\n";
            for (int i = offset; i < (leaderboard.Entries.GetLength(0) + offset) && i < (offset + 15); i++)
            {
                en = leaderboard.Entries[i - offset];

                if (Digits(en.Rank) < digitR)
                    str += "0";

                str += en.Rank + ".   "
                    + ScoreToString(en.Score, type, lb.Character)
                    + "\t" + en.Player + "\n";
            }
            return str;

        }

    }

}
