using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;

namespace Discord
{
    public class Client
    {
        //https://github.com/SinisterRectus/Discordia/blob/master/discordia/client/Client.lua
        // https://github.com/RogueException/Discord.Net/blob/7b99c6003d09783269430fb5a0f7265850b766f8/src/Discord.Net.WebSocket/Net/DefaultWebSocketClient.cs
        /*
         * options (token)
         * api
         * socket
         * user(s)
         * guilds
         * channels
         */
        static public void DoThing()
        {
            using (var ws = new WebSocket("ws://echo.websocket.org"))
            {
                ws.OnMessage += (sender, e) =>
                    Console.WriteLine("Laputa says: " + e.Data);

                ws.Connect();
                ws.Send("BALUS");
                Console.ReadKey(true);
            }
        }
        //run() - set token to api, connect to gateway
        //stop() - socket.disconnect()
        //connect to 
    }
}
