using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;

namespace Discord
{
    public class Socket
    {

        private WebSocket _socket;
        private string _gateway;
        private string _id;
        private Client _client;
        private int _intervals;
        private System.Timers.Timer _timer;

        public Socket(string id, string gateway, Client client)
        {
            _id = id;
            _gateway = gateway;
            _client = client;
            _intervals = 1000;
            _timer = new System.Timers.Timer(_intervals);
        }
    }
}
