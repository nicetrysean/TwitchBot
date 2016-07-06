using System;
using System.IO;
using System.Windows.Forms;
using WMPLib;

namespace TwitchBot
{
    public class Program
    {
        private static ConfigurationReader _config;
        private static Chat _chat;
        private static IrcClient _irc;
        private static WindowsMediaPlayer _mediaPlayer;

        public static void Main(string[] args)
        {
            _config = new ConfigurationReader("credentials.json", "commands.json", "messages.json");

            if (_config.Items == null)
            {
                Console.WriteLine(" ██████╗  ██████╗  ██████╗ ██████╗ ██╗     ██╗   ██╗ ██████╗██╗  ██╗   \r\n██╔════╝ ██╔═══██╗██╔═══██╗██╔══██╗██║     ██║   ██║██╔════╝██║ ██╔╝   \r\n██║  ███╗██║   ██║██║   ██║██║  ██║██║     ██║   ██║██║     █████╔╝    \r\n██║   ██║██║   ██║██║   ██║██║  ██║██║     ██║   ██║██║     ██╔═██╗    \r\n╚██████╔╝╚██████╔╝╚██████╔╝██████╔╝███████╗╚██████╔╝╚██████╗██║  ██╗██╗\r\n ╚═════╝  ╚═════╝  ╚═════╝ ╚═════╝ ╚══════╝ ╚═════╝  ╚═════╝╚═╝  ╚═╝╚═╝");
                while (true)
                {
                    if (Console.KeyAvailable) return;
                }
            }

            foreach (var text in _config.Texts)
            {
                Parser.UserDefinedGenerations.Add(text.Key, new ShuffleBag<string>(text.Value));
            }

            Console.Write(" ██████╗ ██████╗ ███████╗ █████╗ ████████╗         ██╗ ██████╗ ██████╗ \r\n██╔════╝ ██╔══██╗██╔════╝██╔══██╗╚══██╔══╝         ██║██╔═══██╗██╔══██╗\r\n██║  ███╗██████╔╝█████╗  ███████║   ██║            ██║██║   ██║██████╔╝\r\n██║   ██║██╔══██╗██╔══╝  ██╔══██║   ██║       ██   ██║██║   ██║██╔══██╗\r\n╚██████╔╝██║  ██║███████╗██║  ██║   ██║       ╚█████╔╝╚██████╔╝██████╔╝\r\n ╚═════╝ ╚═╝  ╚═╝╚══════╝╚═╝  ╚═╝   ╚═╝        ╚════╝  ╚═════╝ ╚═════╝ \n");
            _irc = new IrcClient("irc.chat.twitch.tv", 6667, _config.User.Username, _config.User.Password);
            _irc.JoinRoom(_config.User.Channel);

            _chat = new Chat(_irc, _config.Items.ToArray());
            
            _mediaPlayer = new WindowsMediaPlayer();
            _mediaPlayer.MediaError += MediaError;
            _mediaPlayer.PlayStateChange += MediaPlayerChange;

            while(true)
            {
                if (!_irc.Connected) _irc.Reconnect();
                _chat.Update();
            }
        }

        private static void MediaPlayerChange(int newState)
        {
            if (_mediaPlayer.playState == WMPPlayState.wmppsStopped)
            {
                _mediaPlayer.close();
            }
        }

        private static void MediaError(object pMediaObject)
        {
            MessageBox.Show("Cannot play media file. :~(");
        }

        public static void PlayAudio(string file)
        {
            if (File.Exists(file))
            {
                _mediaPlayer.URL = file;
                _mediaPlayer.controls.play();
            }
            else
            {
                Console.WriteLine("COULD NOT FIND " + file);
            }
        }
    }
}
