using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Globalization;
using Discord;
using Discord.Commands;

namespace ToofzBot
{
    class Program
    {

        public static Config config;

        static void Main(string[] args)
        {
            Leaderbot.lbInfo = Leaderbot.ParseIndex();
            Leaderbot.LoadNames();
            config = Config.ReadConfig();
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
            //x.HelpMode = HelpMode.Public;
        });

            CreateCommands();

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

            cService.CreateCommand("leaderbot")
                .Parameter("arg", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    var toReturn = CommandHandler.LeaderbotCommand(e.GetArg(0));
                    await e.Channel.SendMessage("```" + toReturn + "```");
                });
            cService.CreateCommand("help")
                .Parameter("arg", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    var toReturn = CommandHandler.LeaderbotCommand("help");
                    await e.Channel.SendMessage("```" + toReturn + "```");
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
            Console.WriteLine($"[{DateTime.Now.ToString(new CultureInfo("fr-FR"))}] [{e.Severity}] [{e.Source}] [{e.Message}]");
        }

    }
}
