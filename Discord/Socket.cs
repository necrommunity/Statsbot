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

    public class SocketMessageEventArgs : EventArgs
    {
        public string Message { get; internal set; }
    }

    public class SocketClosedEventArgs : EventArgs
    {
        public string Reason { get; internal set; }
        public int Code { get; internal set; }
        public bool WasClean { get; internal set; }
    }

    public class SocketErrorEventArgs : EventArgs
    {
        public Exception Exception { get; internal set; }
        public string Message { get; internal set; }
    }

    public class Socket
    {

        private WebSocket socket;

        public string URL { get; set; }
        public bool IsAlive
        {
            get
            {
                return socket != null ? socket.IsAlive : false;
            }
        }

        public event EventHandler<SocketMessageEventArgs> MessageReceived;
        public event EventHandler<SocketClosedEventArgs> SocketClosed;
        public event EventHandler<EventArgs> SocketOpened;
        public event EventHandler<SocketErrorEventArgs> SocketError;

        public Socket(string url)
        {
            URL = url;

            socket = new WebSocket(url);

            HookupEvents();
        }

        private void HookupEvents()
        {
            socket.OnMessage += (sender, e) =>
            {
                SocketMessageEventArgs args = new SocketMessageEventArgs
                {
                    Message = e.Data
                };
                MessageReceived?.Invoke(this, args);
            };

            socket.OnError += (sender, e) =>
            {
                SocketErrorEventArgs args = new SocketErrorEventArgs
                {
                    Exception = e.Exception,
                    Message = e.Message
                };
                SocketError?.Invoke(this, args);
            };

            socket.OnClose += (sender, e) =>
            {
                SocketClosedEventArgs args = new SocketClosedEventArgs
                {
                    Code = e.Code,
                    Reason = e.Reason,
                    WasClean = e.WasClean
                };
                SocketClosed?.Invoke(this, args);
            };

            socket.OnOpen += (sender, e) =>
            {
                SocketOpened?.Invoke(this, null);
            };
        }

        public void Connect()
        {
            socket.Connect();
        }

        public void Close()
        {
            socket.Close();
        }

        public void Send(string data)
        {
            socket.Send(data);
        }

        public void Send(byte[] data)
        {
            socket.Send(data);
        }
    }
}
