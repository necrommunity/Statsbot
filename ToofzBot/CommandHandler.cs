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

    public class CommandHandler
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

        public static string ScoreToString(int score, RunType type, Character character)
        {
            string s = "";
            switch (type)
            {
                case RunType.Score:
                    for (int i = Digits(score); i < 7; i++)
                    {
                        s += " ";
                    }
                    return (s + score.ToString());

                case RunType.Speed:

                    score = 100000000 - score;
                    TimeSpan t = TimeSpan.FromMilliseconds(score);
                    if (score >= 3600000)
                        return (t.ToString(@"h\:mm\:ss\.ff"));
                    return ("  " + t.ToString(@"mm\:ss\.ff"));

                case RunType.Deathless:
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

        public static string LeaderbotCommand(string q)
        {

            if (q.Contains("penguin"))
                return ("ᕕ(' >')ᕗᕕ(' >')ᕗᕕ(' >')ᕗ" + "\npls no bulli");

            if (q.Contains("​")) // gets rid of invisible space
                q = q.Replace("​", null);

            q = q.ToLower();

            string str = q.Split(new[] { ' ' })[0];

            if (str.StartsWith("help"))
            {
                q = q.Replace("help ", null);
                return HelpCommand(q);
            }
            if (str.StartsWith("info"))
                return ("ToofzBot Leaderbot v0.79. Type \".leaderbot help\" for help.");

            if (q.StartsWith("dove longplay"))
                return (DoveLongplay(true));
            if (q.StartsWith("classic dove longplay"))
                return (DoveLongplay(false));

            return (Search(q));
        }


        public static string HelpCommand(string q)
        {
            if (q.StartsWith("leaderboard"))
                return ("Displays a leaderboard."
                    + "\nType \".leaderbot <character>: <category>\" to see a leaderboard."
                    + "\nAdd \"classic\" before the character name to see results without Amplified."
                    + "\nAdd \"&<rank>\" to see the result starting at the specified offset.");
            return ("Leaderbot is a bot which retrieves Crypt of the Necrodancer leaderboards."
                + "\nUse \"search\", \"leaderboard\", or \"help <command>\" for more information."
                + "\nPing Naymin#5067 for questions and bug reports.");
        }


        public static string Search(string q)
        {

            if (!q.Contains(":"))
                return ("Please enter a specific leaderboard in the form of \"<character>: <category> &<offset>\".");

            Leaderboard lb = new Leaderboard();

            string character = q.Split(new[] { ':' })[0];
            if (character.Contains("classic"))
            {
                lb.Amplified = false;
                character = character.Replace("classic", null);
            }
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

            if (type.Contains("seeded"))
            {
                lb.Seeded = true;
                type = type.Replace("seeded", null);
            }

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

            LeaderboardInfo req = new LeaderboardInfo();
            foreach (LeaderboardInfo board in Leaderbot.lbInfo)
            {
                if (board.Leaderboard.isEqual(lb))
                {
                    req = board;
                    break;
                }
            }

            if (req.Id == null)
                return ("Please enter a valid leaderboard.");

            List<Entry> entries = Leaderbot.ParseLeaderboard(req.Id, offset);
            Leaderbot.CheckNames(entries);

            if (entries.Count == 0)
                return ("No entries found for " + req.DisplayName + " ( offset = " + offset + " ).");

            string str = "";
            int digitR = Digits(offset + 15);

            str += "Displaying results for " + req.DisplayName + "\n\n";
            foreach (Entry en in entries)
            {
                if (Digits(en.Rank) < digitR)
                    str += "0";

                str += en.Rank + ".   "
                    + ScoreToString(en.Score, req.Leaderboard.Type, req.Leaderboard.Char)
                    + "\t" + en.ProfileName + "\n";
            }
            return str;

        }

        public static string DoveLongplay(bool amplified)
        {
            LeaderboardInfo lb = new LeaderboardInfo();
            foreach (LeaderboardInfo board in Leaderbot.lbInfo)
            {
                if (amplified && board.Id == 1694557.ToString()) // Dove Speed (Amplified)
                {
                    lb = board;
                    break;
                }
                if (!amplified && board.Id == 741329.ToString()) // Classic
                {
                    lb = board;
                    break;
                }
            }
            int offset = lb.EntryCount - 15;
            List<Entry> entries = Leaderbot.ParseLeaderboard(lb.Id, offset);
            entries.Reverse();
            Leaderbot.CheckNames(entries);

            int digitR = 2;
            string str = "";
            str += "Displaying results for the Dove Longplay";
            if (amplified)
                str += " (Amplified)";
            str += "\n\n";
            for (int i = 1; i < entries.Count + 1; i++)
            {
                if (Digits(i) < digitR)
                    str += "0";
                str += i + ".   "
                    + ScoreToString(entries[i - 1].Score, lb.Leaderboard.Type, lb.Leaderboard.Char)
                    + "\t" + entries[i - 1].ProfileName + "\n";
            }
            return str;
        }
    }

}
