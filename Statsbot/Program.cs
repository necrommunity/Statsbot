using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Globalization;
using Discord;
using Discord.Commands;

//todo:
//order records by count
//include everything in help

namespace Statsbot
{
    class Program
    {

        public static Config config;
        public static Database database;
        public static Racebot racebot;

        static void Main(string[] args)
        {
            new Program().Start();
        }

        private DiscordClient client; //bot configured by following https://youtu.be/ey8woPqvRaI

        public void Start()
        {

            client = new DiscordClient(x =>
            {
                x.AppName = "Statsbot";
                x.LogLevel = LogSeverity.Info;
                x.LogHandler = Log;
            });

            client.UsingCommands(x =>
            {
                x.AllowMentionPrefix = true;
                x.CustomPrefixHandler = (m) =>
                {
                    if (m.RawText.StartsWith(".statsbot"))
                        return 9;
                    if (m.RawText.StartsWith(".sb"))
                        return 3;
                    if (m.Channel.IsPrivate)
                        return 0;
                    return -1;
                };
                x.ErrorHandler += UnknownCommand;
            });

            CreateCommands();

            config = Config.ReadConfig();
            database = Database.ReadConfig();
            XmlParser.lbInfo = XmlParser.ParseIndex();

            if (config.DiscordToken == "" || config.SteamKey == "")
            {
                Console.Write("Please make sure the bot's discord token and steam api key in Config.json are entered correctly before launch.");
                Console.ReadKey();
                return;
            }

            if (database.Server == "")
            {
                Console.Write("Please make sure the Necrobot's database credentials in Database.json are entered correctly before launch.");
                Console.ReadKey();
                return;
            }
            racebot = new Racebot();

            client.ExecuteAndWait(async () => await client.Connect(config.DiscordToken, TokenType.Bot));

        }

        public void CreateCommands()
        {
            var cService = client.GetService<CommandService>();
            cService.CreateCommand("version")
                .Parameter("arg", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("```Statsbot v0.79. Type \".statsbot help\" for a list of commands.```");
                });
            cService.CreateCommand("help")
                .Parameter("arg", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("```" + CommandHandler.Help(e.GetArg(0).ToLower()) + "```");
                });
            cService.CreateCommand("search")
                .Alias(new string[] { "player", "toofz", "s" })
                .Parameter("arg", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("```" + CommandHandler.Search(e.GetArg(0).ToLower()) + "```");
                });
            cService.CreateCommand("records")
                .Alias(new string[] { "stats" })
                .Parameter("arg", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("```" + CommandHandler.Records(e.GetArg(0).ToLower()) + "```");
                });
            cService.CreateCommand("leaderboard")
                .Alias(new string[] { "lb" })
                .Parameter("arg", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("```" + CommandHandler.Leaderboard(e.GetArg(0).ToLower()) + "```");
                });
            cService.CreateCommand("necrobot")
                .Alias(new string[] { "races" })
                .Parameter("arg", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("```" + racebot.DisplayResults(e.User.Name, e.GetArg(0).ToLower()) + "```");
                });
            cService.CreateCommand("penguin")
                .Parameter("arg", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("```ᕕ(' >')ᕗᕕ(' >')ᕗᕕ(' >')ᕗ" + "\npls no bulli```");
                });
            //cService.CreateCommand("rename")
            //    .Parameter("arg", ParameterType.Unparsed)
            //    .Do(async (e) =>
            //    {
            //        await client.CurrentUser.Edit(username: "Statsbot");
            //    });
        }

        public void UnknownCommand(object sender, CommandErrorEventArgs e)
        {
            e.Channel.SendMessage("```Unknown command \"" + e.Message.RawText[0] + "\". Type \".statsbot help\" for a list of commands.\n"
                        + "Tip: you can use commands in private messages.```");
        }

        public void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine($"[{DateTime.Now.ToString(new CultureInfo("fr-FR"))}] [{e.Severity}] [{e.Source}] [{e.Message}]");
        }

    }
}
