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
                x.PrefixChar = '.';
                //x.HelpMode = HelpMode.Public;
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

            if(database.Server == "")
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

            cService.CreateCommand("statsbot")
                .Parameter("arg", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    var toReturn = CommandHandler.ParseRequest(e.GetArg(0), e.User);
                    await e.Channel.SendMessage("```" + toReturn + "```");
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

        public void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine($"[{DateTime.Now.ToString(new CultureInfo("fr-FR"))}] [{e.Severity}] [{e.Source}] [{e.Message}]");
        }

    }
}
