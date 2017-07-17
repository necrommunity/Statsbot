using System;
using System.IO;
using System.Text;
using SteamKit2;

namespace SteamLB
{
    class Program
    {
        static SteamClient steamClient;
        static CallbackManager manager;

        static SteamUser steamUser;
        static SteamUserStats steamUserStats;

        static bool isRunning;

        static bool loggedIn;

        static string user = Statsbot.Program.config.Username, pass = Statsbot.Program.config.Password;

        static System.Collections.Generic.List<Statsbot.SteamEntry> steamEntries;

        public static System.Collections.Generic.List<Statsbot.SteamEntry> FetchEntries(string lbid, int offset)
        {
            Run(lbid, offset);
            steamClient.Disconnect();
            return steamEntries;
        }

        static void Run(string lbid, int offset)
        {

            steamClient = new SteamClient(System.Net.Sockets.ProtocolType.Tcp);
            manager = new CallbackManager(steamClient);

            steamUser = steamClient.GetHandler<SteamUser>();
            steamUserStats = steamClient.GetHandler<SteamUserStats>();

            manager.Subscribe<SteamClient.ConnectedCallback>(OnConnected);
            manager.Subscribe<SteamClient.DisconnectedCallback>(OnDisconnected);
            manager.Subscribe<SteamUser.LoggedOnCallback>(OnLoggedOn);
            manager.Subscribe<SteamUser.LoggedOffCallback>(OnLoggedOff);

            isRunning = true;

            //Console.WriteLine($"[{DateTime.Now.ToString(new System.Globalization.CultureInfo("fr-FR"))}]Connecting to Steam...");

            loggedIn = false;

            SteamDirectory.Initialize().Wait();

            steamClient.Connect();

            while (!loggedIn)
            {
                manager.RunWaitCallbacks(TimeSpan.FromSeconds(1));
                if (!isRunning)
                {
                    return;
                }
            }
            DownloadLeaderboardRange(lbid, offset, offset + 9);
        }

        static EResult DownloadLeaderboardRange(string lbid, int first, int last)
        {
            //Console.WriteLine($"Downloading entries {first} to {last}...");

            var queryLeaderboardEntriesJob = steamUserStats.GetLeaderboardEntries(247080, int.Parse(lbid), first, last, ELeaderboardDataRequest.Global);

            while (queryLeaderboardEntriesJob.GetAwaiter().IsCompleted)
            {
                manager.RunWaitCallbacks(TimeSpan.FromSeconds(1));
                if (!isRunning)
                {
                    return EResult.Fail;
                }
            }

            var result = queryLeaderboardEntriesJob.GetAwaiter().GetResult();

            if (result.Result == EResult.OK)
            {
                steamEntries = new System.Collections.Generic.List<Statsbot.SteamEntry>();
                foreach (var entry in result.Entries)
                {
                    Statsbot.SteamEntry en = new Statsbot.SteamEntry();
                    en.Steamid = entry.SteamID.ConvertToUInt64().ToString();
                    en.Rank = entry.GlobalRank;
                    en.Score = entry.Score;
                    en.UgcID = entry.UGCId.ToString();
                    steamEntries.Add(en);
                }
            }
            else
            {
                Console.Error.WriteLine($"Failed to download leaderboard section! Error code: {result.Result.ToString()}");
            }
            return result.Result;
        }

        static void OnConnected(SteamClient.ConnectedCallback callback)
        {
            if (callback.Result != EResult.OK)
            {
                Console.WriteLine("Unable to connect to Steam: {0}", callback.Result);

                isRunning = false;
                return;
            }

            Console.WriteLine($"[{DateTime.Now.ToString(new System.Globalization.CultureInfo("fr-FR"))}] Connected to Steam! Logging in '{user}'...");

            steamUser.LogOn(new SteamUser.LogOnDetails
            {
                Username = user,
                Password = pass,
            });
        }

        static void OnDisconnected(SteamClient.DisconnectedCallback callback)
        {
            Console.WriteLine("Disconnected from Steam");

            isRunning = false;
        }

        static void OnLoggedOn(SteamUser.LoggedOnCallback callback)
        {
            if (callback.Result != EResult.OK)
            {
                if (callback.Result == EResult.AccountLogonDenied)
                {
                    Console.WriteLine("Unable to logon to Steam: This account is SteamGuard protected.");

                    isRunning = false;
                    return;
                }

                Console.WriteLine($"Unable to logon to Steam: {callback.Result.ToString()} / {callback.ExtendedResult}");

                isRunning = false;
                return;
            }

            //Console.WriteLine("Successfully logged on!");
            loggedIn = true;
        }

        static void OnLoggedOff(SteamUser.LoggedOffCallback callback)
        {
            Console.WriteLine("Logged off of Steam: {callback.Result.ToString()}");
        }
    }
}
