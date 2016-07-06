using System.IO;
using System.Net.Sockets;

namespace TwitchBot
{
    internal class IrcClient
    {
        private readonly string _username;
        private string _channel;

        private string _lastUsedIp;
        private int _lastUsedPort;

        private StreamReader _inputStream;
        private StreamWriter _outputStream;
        private TcpClient _tcpClient;

        public bool Connected => _tcpClient.Connected;

        public IrcClient(string ip, int port, string username, string password)
        {
            _username = username;
            Connect(ip, port);
            _outputStream.WriteLine("PASS " + password);
            _outputStream.WriteLine("NICK " + username);
            _outputStream.WriteLine("USER {0} 8 * :{0}", username);
            _outputStream.Flush();
        }

        private void Connect(string ip, int port)
        {
            _lastUsedIp = ip;
            _lastUsedPort = port;

            _tcpClient = new TcpClient(ip, port);
            _inputStream = new StreamReader(_tcpClient.GetStream());
            _outputStream = new StreamWriter(_tcpClient.GetStream());
        }

        public void JoinRoom(string channel)
        {
            _channel = channel;
            _outputStream.WriteLine("JOIN " + channel);
            _outputStream.Flush();
        }

        public void SendIrcMessage(string message)
        {
            _outputStream.WriteLine(message);
            _outputStream.Flush();
        }

        public void SendChatMessage(string message)
        {
            SendIrcMessage(string.Format(":{0}!{0}@{0}.tmi.twitch.tv PRIVMSG {1} :{2}", _username, _channel, message));
        }

        public void SendWhisper(string user, string message)
        {
            SendIrcMessage(string.Format(":{0}!{0}@{0}.tmi.twitch.tv PRIVMSG {1} :/w {3} {2}", _username, _channel,
                message, user));
        }

        public void Reconnect()
        {
            _tcpClient.Connect(_lastUsedIp, _lastUsedPort);
        }

        public string ReadMessage()
        {
            return _inputStream.ReadLine();
        }
    }
}
