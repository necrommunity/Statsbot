using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Statsbot
{

    public class RecordsResponse
    {
        public Records Playerstats { get; set; }
    }

    public class Records
    {
        public static Dictionary<string, string> Names { get; set; } = new Dictionary<string, string>()
        { { "NumDeaths",  "deaths"}, { "NumShopkeeperKills", "freddies" },{ "NumGreenBatKills", "bats" }, {"NumZone1Completions", "z1" },
            { "NumZone2Completions", "z2" },{"NumZone3Completions", "z3" },{"NumZone4Completions", "z4" },{"NumSub8CadenceSpeedruns", "speedruns" },
            {"NumDailyChallengeCompletions", "dailies" }, {"NumHardcoreCompletionsCadence", "Cadence" }, {"NumHardcoreCompletionsAria", "Aria" },
            {"NumHardcoreCompletionsBard", "Bard" }, {"NumHardcoreCompletionsBolt", "Bolt" }, {"NumHardcoreCompletionsMonk", "Monk" },
            { "NumHardcoreCompletionsDove", "Dove" },{"NumHardcoreCompletionsEli", "Eli" }, { "NumHardcoreCompletionsMelody", "Melody" },
            { "NumHardcoreCompletionsDorian", "Dorian" }, {"NumHardcoreCompletionsCoda", "Coda" }, { "NumHardcoreCompletionsNocturna", "Nocturna" },
            { "NumAllCharsCompletions", "All" }, {"NumAriaLowPercentCompletions", "ariaLow" },{"NumAllCharsLowPercentCompletions", "lowest" }, {"story", "Story" }, {"diamond", "Diamond" } };

        public string SteamID { get; set; }
        public Stat[] Stats { get; set; }
        public Dictionary<string, int> Organized { get; set; } = new Dictionary<string, int>();

        public void Organize()
        {
            foreach (string s in Names.Values)
            {
                Organized[s] = 0;
            }
            foreach (Stat s in Stats)
            {
                if (Names.ContainsKey(s.Name))
                    Organized[Names[s.Name]] = s.Value;
            }
        }
    }

    public class Stat
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }


}
