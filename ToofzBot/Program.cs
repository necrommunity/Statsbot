using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Discord;
using Discord.Commands;

namespace ToofzBot
{
    class Program
    {

        public static Config config;

        static void Main(string[] args)
        {
            new Program().Start();
        }

        private DiscordClient client; //bot configured by following https://youtu.be/ey8woPqvRaI

        public void Start()
        {

            client = new DiscordClient(x =>
            {
                    x.AppName = "ToofzBot";
                    x.LogLevel = LogSeverity.Info;
                    x.LogHandler = Log;
            });

            client.UsingCommands(x =>
            {
                x.AllowMentionPrefix = true;
                x.PrefixChar = '.';
                x.HelpMode = HelpMode.Public;
            });

            CreateCommands();

            config = Config.ReadConfig();

            if (config.DiscordToken == "" || config.SteamKey == "")
            {
                Console.Write("Please enter the bot's Discord token and the steam api key in Config.json before launch.");
                Console.ReadKey();
                return;
            }

            client.ExecuteAndWait(async () => await client.Connect(config.DiscordToken, TokenType.Bot));

        }

        public void CreateCommands()
        {
            var cService = client.GetService<CommandService>();

            cService.CreateCommand("ping")
                .Description("Returns 'pong'.")
                .Parameter("arg", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    var toReturn = "pong" + e.GetArg(0);
                    await e.Channel.SendMessage(toReturn);
                });

            cService.CreateCommand("search")
                .Description("```Searches for a player. Type \".search <name>\" to see a list of results, \".search <name>: <category>\" to see results for a specific run.\nSteamID can be used instead of a name ( for example:  \".search #245356: seeded score\" ).```")
                .Parameter("arg", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    var toReturn = CommandHandler.SearchName(e.GetArg(0));
                    await e.Channel.SendMessage(toReturn);
                });
            cService.CreateCommand("leaderboard")
                .Description("Displays a leaderboard. The format is \".leaderboard <character>: <category>.  Add &<rank> to see the result from the specified offset.")
                .Parameter("arg", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    var toReturn = CommandHandler.SearchLeaderboard(e.GetArg(0));
                    await e.Channel.SendMessage(toReturn);
                });
            //cService.CreateCommand("test")
            //    .Description("aaa.")
            //    .Parameter("arg", ParameterType.Unparsed)
            //    .Do(async (e) =>
            //    {
            //        var toReturn = CommandHandler.SearchLeaderboard(e.GetArg(0));
            //        await e.Channel.SendMessage(toReturn);
            //    });
        }

        public void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine($"[{e.Severity}] [{e.Source}] [{e.Message}]");
        }

    }
}
