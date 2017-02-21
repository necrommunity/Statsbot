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
                x.AppName = "Toofzbot";
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

            racebot = new Racebot();
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

            cService.CreateCommand("toofz")
                .Parameter("arg", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    var toReturn = CommandHandler.ToofzCommand(e.GetArg(0));
                    await e.Channel.SendMessage("```" + toReturn + "```");
                });
            //cService.CreateCommand("rename")
            //    .Parameter("arg", ParameterType.Unparsed)
            //    .Do(async (e) =>
            //    {
            //        await client.CurrentUser.Edit(username: "name");
            //    });
            cService.CreateCommand("stats")
                .Parameter("arg", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    var toReturn = CommandHandler.SearchPlayerStats(e.GetArg(0));
                    await e.Channel.SendMessage("```" + toReturn + "```");
                });
            cService.CreateCommand("penguin")
                .Parameter("arg", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("```ᕕ(' >')ᕗᕕ(' >')ᕗᕕ(' >')ᕗ" + "\npls no bulli```");
                });
            cService.CreateCommand("condorbotstatus")
                .Alias(new string[] { "register", "timezone" })
                .Parameter("arg", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    bool cbot = (e.Channel.Name == "season5" && e.Server.GetUser("condorbot", 0).Status != UserStatus.Online);
                    await e.Channel.SendMessage("`Condorbot is currently offline. Please register later.`");
                });
            cService.CreateCommand("necrobot")
                .Parameter("arg", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    var toReturn = racebot.DisplayResults(e.User.Id.ToString(), e.GetArg(0));
                    await e.Channel.SendMessage("```" + toReturn + "```");
                });
        }

        public void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine($"[{DateTime.Now.ToString(new CultureInfo("fr-FR"))}] [{e.Severity}] [{e.Source}] [{e.Message}]");
        }

    }
}
