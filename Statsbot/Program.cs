using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Globalization;
using Discord;
using Discord.Commands;


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
                    if (m.Server != null && m.Server.Id == 83287148966449152 && m.Channel.Id != 296636142210646016) //main server limit to botspam
                        return -1;
                    if (m.RawText.StartsWith(".statsbot"))
                        return 9;
                    if (m.RawText.StartsWith(".sb"))
                        return 3;
                    if (m.RawText.StartsWith(".") && (m.Channel.IsPrivate || m.Server.Id != 214389515417026561)) //prevent conflict with inc's bot
                        return 1;
                    return -1;
                };
                x.ErrorHandler += UnknownCommand;
            });

            CreateCommands();

            config = Config.ReadConfig();
            database = Database.ReadConfig();
            //XmlParser.lbInfo = XmlParser.ParseIndex();
            XmlParser.ReadLeaderboards();

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

            if (!System.IO.File.Exists(@"Stats.json"))
            {
                Console.Write("Please make sure the file \"Stats.json\" is located in the bin folder.");
                Console.ReadKey();
                return;
            }

            racebot = new Racebot();

            //while (true) //debug mode essentially
            //{
            //    string arg = Console.ReadLine();
            //    Console.WriteLine(CommandHandler.Leaderboard(arg));
            //}

            client.ExecuteAndWait(async () => await client.Connect(config.DiscordToken, TokenType.Bot));

        }



        public void CreateCommands()
        {
            var cService = client.GetService<CommandService>();
            cService.CreateCommand("version")
                .Parameter("arg", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("```Statsbot v0.82. Type \".statsbot help\" for a list of commands.```");
                });
            cService.CreateCommand("help")
                .Parameter("arg", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("```" + CommandHandler.Help(e.GetArg(0).ToLower()) + "```");
                });
            cService.CreateCommand("search")
                .Alias(new string[] { "s" })
                .Parameter("arg", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("```" + CommandHandler.SearchPlayers(ParseArgs(e.GetArg(0), e.User)) + "```");
                });
            cService.CreateCommand("speed")
                .Parameter("arg", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("```" + CommandHandler.PlayerScores(ParseArgs(e.GetArg(0), e.User), RunType.Speed) + "```");
                });
            cService.CreateCommand("score")
                .Parameter("arg", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("```" + CommandHandler.PlayerScores(ParseArgs(e.GetArg(0), e.User), RunType.Score) + "```");
                });
            cService.CreateCommand("deathless")
                .Parameter("arg", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("```" + CommandHandler.PlayerScores(ParseArgs(e.GetArg(0), e.User), RunType.Deathless) + "```");
                });
            cService.CreateCommand("records")
                .Alias(new string[] { "stats", "record" })
                .Parameter("arg", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("```" + CommandHandler.Records(ParseArgs(e.GetArg(0), e.User)) + "```");
                });
            cService.CreateCommand("leaderboard")
                .Alias(new string[] { "lb" })
                .Parameter("arg", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("```" + CommandHandler.Leaderboard(e.GetArg(0).ToLower()) + "```");
                });
            cService.CreateCommand("necrobot")
                .Alias(new string[] { "race", "races" })
                .Parameter("arg", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("```" + racebot.DisplayResults(e.GetArg(0), e.User) + "```");
                });
            cService.CreateCommand("penguin")
                .Parameter("arg", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("```ᕕ(' >')ᕗᕕ(' >')ᕗᕕ(' >')ᕗ" + "\npls no bulli```");
                });
            cService.CreateCommand("secret")
                .Parameter("arg", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    if (e.User.Id == 86612976529838080 || e.User.Id == 155424249211781120)
                        await e.Channel.SendMessage("```https://youtu.be/dQw4w9WgXcQ```");
                });

            //cService.CreateCommand("rename")
            //    .Parameter("arg", ParameterType.Unparsed)
            //    .Do(async (e) =>
            //    {
            //        await client.CurrentUser.Edit(username: "Statsbot");
            //    });
        }

        public static string ParseArgs(string args, User user)
        {
            if (args.Length == 0)
                return (user.Name);
            else
                return (args.ToLower());
        }

        public void UnknownCommand(object sender, CommandErrorEventArgs e)
        {
            string args = e.Message.RawText;
            if (args.StartsWith(".statsbot") || args.StartsWith(".sb"))
            {
                if (args.Contains(' '))
                    e.Channel.SendMessage("```Unknown command \"" + args.Split(' ')[1] + "\". Type \".statsbot help\" for a list of commands.```");
                else
                    e.Channel.SendMessage("```Please enter a command. Type \".statsbot help\" for a list of commands.```");
            }
        }

        public void Log(object sender, LogMessageEventArgs e)
        {
            if (!(e.Message == "Unknown message type: MESSAGE_REACTION_REMOVE" || e.Message == "Unknown message type: MESSAGE_REACTION_ADD"))
                Console.WriteLine($"[{DateTime.Now.ToString(new CultureInfo("fr-FR"))}] [{e.Severity}] [{e.Source}] [{e.Message}]");
        }

    }
}
