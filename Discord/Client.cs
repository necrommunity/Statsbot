using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Discord
{
    public class Client
    {
        //options (token)
        //api
        private Socket ws;
        //user(s)
        //guilds
        //channels
        //logger

        public event EventHandler<EventArgs> SocketOpened;

        string token = "Mjc4MTQ1MTIzNDc5NzE1ODQw.DI8xUw.wbjiJ-_jDrGNqzRnFgG3d7pio2E";
        string url = "https://discordapp.com/api/v6/gateway";

        public void Connect()
        {
        }

    }
}
