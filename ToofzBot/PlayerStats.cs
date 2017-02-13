using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ToofzBot
{

    public class StatsResponse
    {
        public PlayerStats Playerstats { get; set; }

        public static PlayerStats GetPlayerStats(string id) // https://developer.valvesoftware.com/wiki/Steam_Web_API#GetUserStatsForGame_.28v0002.29
        {
            string response;
            using (WebClient client = new WebClient())
            {
                response = client.DownloadString("http://api.steampowered.com/ISteamUserStats/GetUserStatsForGame/v0002/?appid=247080&key=" + Program.config.SteamKey + "&steamid=" + id);
            }
            PlayerStats toReturn = JsonConvert.DeserializeObject<StatsResponse>(response).Playerstats;
            return (toReturn);
        }
    }

    public class PlayerStats
    {
        public enum StatsList { Deaths, Freddies, Bats, Z1, Z2, Z3, Z4, Speedruns, Dailies, Cadence, Aria, Bard, Bolt, Monk, Dove, Eli, Melody, Dorian, Coda, Nocturna, All, AriaLow, Lowest };
        public static string[] Characters = new string[12];
        public string SteamID { get; set; }
        public Stat[] Stats { get; set; }
        public Dictionary<string, int> Organized { get; set; } = new Dictionary<string, int>();

        public void OrganizeStats()
        {
            foreach(string s in Enum.GetNames(typeof(StatsList)))
            {
                Organized[s] = 0;
            }
            foreach (Stat s in this.Stats)
            {
                switch (s.Name)
                {
                    case ("NumDeaths"):
                        Organized["Deaths"] = s.Value;
                        break;
                    case ("NumShopkeeperKills"):
                        Organized["Freddies"] = s.Value;
                        break;
                    case ("NumGreenBatKills"):
                        Organized["Bats"] = s.Value;
                        break;
                    case ("NumZone1Completions"):
                        Organized["Z1"] = s.Value;
                        break;
                    case ("NumZone2Completions"):
                        Organized["Z2"] = s.Value;
                        break;
                    case ("NumZone3Completions"):
                        Organized["Z3"] = s.Value;
                        break;
                    case ("NumZone4Completions"):
                        Organized["Z4"] = s.Value;
                        break;
                    case ("NumSub8CadenceSpeedruns"):
                        Organized["Speedruns"] = s.Value;
                        break;
                    case ("NumDailyChallengeCompletions"):
                        Organized["Dailies"] = s.Value;
                        break;
                    case ("NumHardcoreCompletionsCadence"):
                        Organized["Cadence"] = s.Value;
                        break;
                    case ("NumHardcoreCompletionsAria"):
                        Organized["Aria"] = s.Value;
                        break;
                    case ("NumAriaLowPercentCompletions"):
                        Organized["AriaLow"] = s.Value;
                        break;
                    case ("NumHardcoreCompletionsBard"):
                        Organized["Bard"] = s.Value;
                        break;
                    case ("NumHardcoreCompletionsBolt"):
                        Organized["Bolt"] = s.Value;
                        break;
                    case ("NumHardcoreCompletionsMonk"):
                        Organized["Monk"] = s.Value;
                        break;
                    case ("NumHardcoreCompletionsDove"):
                        Organized["Dove"] = s.Value;
                        break;
                    case ("NumHardcoreCompletionsEli"):
                        Organized["Eli"] = s.Value;
                        break;
                    case ("NumHardcoreCompletionsMelody"):
                        Organized["Melody"] = s.Value;
                        break;
                    case ("NumHardcoreCompletionsDorian"):
                        Organized["Dorian"] = s.Value;
                        break;
                    case ("NumHardcoreCompletionsCoda"):
                        Organized["Coda"] = s.Value;
                        break;
                    case ("NumHardcoreCompletionsNocturna"):
                        Organized["Nocturna"] = s.Value;
                        break;
                    case ("NumAllCharsCompletions"):
                        Organized["All"] = s.Value;
                        break;
                    case ("NumAllCharsLowPercentCompletions"):
                        Organized["Lowest"] = s.Value;
                        break;
                }
            }
            Array.Copy(Enum.GetNames(typeof(PlayerStats.StatsList)), 9, PlayerStats.Characters, 0, 12);
        }
    }

    public class Stat
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }


}
