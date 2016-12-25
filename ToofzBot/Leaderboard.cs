using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToofzBot
{
    public class Leaderboard
    {
        public int LeaderboardId { get; set; }
        public string Character { get; set; }
        public string Run { get; set; }
    }

    public class Leaderboards
    {
        public List<Leaderboard> Score { get; set; }
        public List<Leaderboard> Speed { get; set; }
        public List<Leaderboard> SeededScore { get; set; }
        public List<Leaderboard> SeededSpeed { get; set; }
        public List<Leaderboard> Deathless { get; set; }

        public Leaderboards(Leaderboard[] lboards)
        {

            Score = new List<Leaderboard>();
            Speed = new List<Leaderboard>();
            SeededScore = new List<Leaderboard>();
            SeededSpeed = new List<Leaderboard>();
            Deathless = new List<Leaderboard>();

            foreach (Leaderboard lb in lboards)
            {
                switch (lb.Run)
                {
                    case "Score":
                        Score.Add(lb);
                        break;
                    case "Speed":
                        Speed.Add(lb);
                        break;
                    case "Seeded Score":
                        SeededScore.Add(lb);
                        break;
                    case "Seeded Speed":
                        SeededSpeed.Add(lb);
                        break;
                    case "Deathless":
                        Deathless.Add(lb);
                        break;
                }
            }
        }

    }
}
