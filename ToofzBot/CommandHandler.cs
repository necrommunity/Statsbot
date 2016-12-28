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
            switch (type)
            {
                case "score":
                case "seeded score":
                    return (score.ToString());

                case "speed":
                case "seeded speed":

                    score = 100000000 - score;
                    TimeSpan t = TimeSpan.FromMilliseconds(score);
                    if (score >= 3600000)
                        return (t.ToString(@"h\:mm\:ss\.ff"));
                    return (t.ToString(@"mm\:ss\.ff"));

                case "deathless":
                    int wins = score / 100;
                    score = score % 100;
                    if (!character.Equals("Aria"))
                        return (wins + " (" + (score / 10 + 1) + "-" + (score % 10 + 1) + ")");
                    else
                        return (wins + " (" + FlipZone(score / 10 + 1) + "-" + (score % 10 + 1) + ")");
                default:
                    return "error";
            }

        }

        public static string ToofzCommand(string q)
        {
            string str = q.Split(new[] { ' ' })[0];
            if (q.Contains("penguin"))
                return ("@ᕕ(' >')ᕗᕕ(' >')ᕗᕕ(' >')ᕗ" + "\npls no bulli");
            if (q.Contains("​")) //invisible space
                q = Regex.Replace(q, ("​"), string.Empty, RegexOptions.Multiline);
            switch (str)
            {
                case "search":
                    q = Regex.Replace(q, "search ", string.Empty, RegexOptions.Multiline);
                    return (SearchName(q));
                case "leaderboard":
                    q = Regex.Replace(q, "leaderboard ", string.Empty, RegexOptions.Multiline);
                    return (SearchLeaderboard(q));
                case "help":
                    q = Regex.Replace(q, "help ", string.Empty, RegexOptions.Multiline);
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
                        + "\nUse \"search\", \"leaderboard\", or \"help <command>\" for more information.");
                default:
                    return ("Unknown command. Use \"search\", \"leaderboard\", or \"help <command>\" for more information.");
            }
        }

        public static string SearchName(string q)
        {
            string str = "";
            bool isID = q.StartsWith("#");
            if (isID)
                q = q.Trim(new[] { '#' });

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

                if (isID)
                    return (SearchId(name, type));
                else
                    return (SearchPlayer(name, type));

            }
        }

        public static string SearchPlayer(string name, string type)
        {

            PlayerResults results = ApiSender.GetPlayers(name);
            if (results.Entries.GetLength(0) == 0)
                return ("Couldn't find results for the name \"" + name + "\".");

            PlayerResult player = results.Entries[0];
            Player idp = ApiSender.GetPlayersId(player.SteamId);

            string str = "";
            int digitS = Digits(idp.Runs[0].Entries[0].Score);

            foreach (PlayerRun pr in idp.Runs)
            {
                if (type.Equals(pr.Run.DisplayName, StringComparison.OrdinalIgnoreCase))
                {
                    str += "Player: " + player.Name + "\n";
                    str += "PlayerID: " + player.SteamId + "\n";
                    str += "Top " + pr.Run.DisplayName + " results\n";
                    foreach (PlayerEntry e in pr.Entries)
                    {
                        str += "\t" + e.Character;
                        for (int i = e.Character.Length; i < 7; i++)
                        {
                            str += " ";
                        }

                        if (pr.Run.Kind == "speed")
                        {
                            if ((100000000 - e.Score) < 3600000)
                                str += "  ";
                        }
                        if (pr.Run.Kind == "deathless" && e.Score < 100)
                            str += " ";
                        else
                        {
                            for (int d = Digits(e.Score); d < digitS; d++)
                            {
                                str += " ";
                            }
                        }
                        str += "\t" + ScoreToString(e.Score, pr.Run.Kind, e.Character)
                                + " / " + e.Rank + "\n";
                    }
                    return str;
                }
            }
            return ("No entries found for the player \"" + player.Name + "\" in the category " + type + ".");
        }

        public static string SearchId(string id, string type)
        {
            SteamUser user = ApiSender.GetSteamUser(id);
            if (user.Steamid == "error")
                return ("Couldn't find results for the steamID \"" + id + "\".");

            Player idp = ApiSender.GetPlayersId(id.ToString());

            string str = "";
            int digitS = Digits(idp.Runs[0].Entries[0].Score);

            foreach (PlayerRun pr in idp.Runs)
            {
                if (type.Equals(pr.Run.DisplayName, StringComparison.OrdinalIgnoreCase))
                {
                    str += "Player: " + user.Personaname + "\n";
                    str += "PlayerID: " + id + "\n";
                    str += "Top " + pr.Run.DisplayName + " results\n";
                    foreach (PlayerEntry e in pr.Entries)
                    {
                        str += "\t" + e.Character;
                        for (int i = e.Character.Length; i < 7; i++)
                        {
                            str += " ";
                        }

                        if (pr.Run.Kind == "speed")
                        {
                            if ((100000000 - e.Score) < 3600000)
                                str += "  ";
                        }
                        if (pr.Run.Kind == "deathless" && e.Score < 100)
                            str += " ";
                        else
                        {
                            for (int d = Digits(e.Score); d < digitS; d++)
                            {
                                str += " ";
                            }
                        }
                        str += "\t" + ScoreToString(e.Score, pr.Run.Kind, e.Character)
                                + " / " + e.Rank + "\n";
                    }
                    return str;
                }
            }
            return ("No entries found for steamID " + id + " in the category " + type + ".");
        }

        public static string SearchLeaderboard(string q)
        {

            if (!q.Contains(":"))
                return ("Please enter a specific leaderboard in the form of \"-leaderboard <character>: <type> &<offset>\".");

            string character = q.Split(new[] { ':' })[0];
            string type = q.Split(new[] { ':' })[1];
            type = type.Trim();
            int offset = 0;
            Leaderboard lb = null;

            if (q.Contains("&"))
            {
                int.TryParse(q.Split(new[] { '&' })[1], out offset);
                type = type.Split(new[] { '&' })[0];
                type = type.TrimEnd(new[] { ' ' });
            }

            if (q.Contains("offset"))
            {
                q = Regex.Replace(q, "=", string.Empty, RegexOptions.Multiline);
                int.TryParse(q.Split(new[] { "offset" }, StringSplitOptions.None)[1], out offset);
                type = type.Split(new[] { "offset" }, StringSplitOptions.None)[0];
                type = type.TrimEnd(new[] { ' ' });
            }

            Leaderboard[] boards = ApiSender.GetLeaderboards();
            foreach (Leaderboard b in boards)
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

            string str = "";
            int digitR = Digits(offset + 15);
            if (leaderboard.Entries.Length == 0)
                return ("No entries found for " + lb.Character + " / " + lb.Run + " ( offset = " + offset + " ).");
            int digitS = Digits(leaderboard.Entries[0].Score);

            if (lb.Run == "Speed" || lb.Run == "Seeded Speed")
            {
                if ((100000000 - leaderboard.Entries[15].Score) > 3600000)
                    digitS = 7;
                else
                    digitS = 6;
            }

            Entry en = null;

            str += "Displaying leaderboard results for " + lb.Character + " / " + lb.Run + " ( offset = " + offset + " )\n\n";
            for (int i = offset; i < (leaderboard.Entries.GetLength(0) + offset) && i < (offset + 15); i++)
            {

                en = leaderboard.Entries[i - offset];

                if (Digits(en.Rank) < digitR)
                    str += "0";
                str += en.Rank + ". \t";

                if (lb.Run == "Speed" || lb.Run == "Seeded speed")
                {
                    if (digitS == 7 && (100000000 - en.Score) < 3600000)
                        str += "  ";
                }
                else
                {
                    for (int d = Digits(en.Score); d < digitS; d++)
                    {
                        str += " ";
                    }
                }
                if (lb.Run == "Deathless" && en.Score >= 100)
                    str += " ";
                str += ScoreToString(en.Score, type, lb.Character);
                str += "\t" + en.Player + "\n";
            }
            return str;
        }

    }

}
