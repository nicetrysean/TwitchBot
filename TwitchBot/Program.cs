using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Timers;
using System.Windows.Forms;
using Microsoft.Win32;
using WMPLib;
using Timer = System.Timers.Timer;

namespace TwitchBot
{
    public class Program
    {
        private static ConfigurationReader _config;
        private static Timer _voteTimer;
        private static readonly Dictionary<string, int> Votes = new Dictionary<string, int>();
        private static readonly List<string> Frozen = new List<string>();
        private static IrcClient _irc;
        private static WMPLib.WindowsMediaPlayer _mediaPlayer;

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
                Parser.Generations.Add(text.Key, new ShuffleBag<string>(text.Value));
            }

            Console.Write(" ██████╗ ██████╗ ███████╗ █████╗ ████████╗         ██╗ ██████╗ ██████╗ \r\n██╔════╝ ██╔══██╗██╔════╝██╔══██╗╚══██╔══╝         ██║██╔═══██╗██╔══██╗\r\n██║  ███╗██████╔╝█████╗  ███████║   ██║            ██║██║   ██║██████╔╝\r\n██║   ██║██╔══██╗██╔══╝  ██╔══██║   ██║       ██   ██║██║   ██║██╔══██╗\r\n╚██████╔╝██║  ██║███████╗██║  ██║   ██║       ╚█████╔╝╚██████╔╝██████╔╝\r\n ╚═════╝ ╚═╝  ╚═╝╚══════╝╚═╝  ╚═╝   ╚═╝        ╚════╝  ╚═════╝ ╚═════╝ \n");
            _irc = new IrcClient("irc.chat.twitch.tv", 6667, _config.User.Username, _config.User.Password);
            _irc.JoinRoom(_config.User.Channel);

            _mediaPlayer = new WindowsMediaPlayer();
            _mediaPlayer.MediaError += MediaError;
            _mediaPlayer.PlayStateChange += MediaPlayerChange;

            _voteTimer = new Timer(120000);
            _voteTimer.Elapsed += ClearVoting;
            while(true)
            {   
                Update();
            }
        }

        private static void MediaPlayerChange(int newState)
        {
            if (_mediaPlayer.playState == WMPPlayState.wmppsStopped)
            {
                _mediaPlayer.close();
            }
        }

        private static void ClearVoting(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            Votes.Clear();
        }

        private static void Update()
        {
            if(!_irc.Connected) _irc.Reconnect();

            var message = _irc.ReadMessage();
            Console.WriteLine(message);

            for (int index = 0; index < _config.Items.Count; index++)
            {
                var item = _config.Items[index];
                if (!message.Contains(item.Command)) continue;

                if (Frozen.Contains(item.Command))
                {
                    if (!string.IsNullOrEmpty(item.MisfireText))
                    {
                        _irc.SendChatMessage(Parser.Parse(item.MisfireText));
                        continue;
                    }
                }

                switch (item.Action)
                {
                    case "Text":
                        _irc.SendChatMessage(Parser.Parse(item.Text));
                        break;
                    case "Audio":
                        PlayAudio($@"{Directory.GetCurrentDirectory()}\Audio\{item.FileName}");
                        break;
                    case "Vote":
                        if (Votes.ContainsKey(item.Command))
                        {
                            if (item.VotesRequired <= (Votes[item.Command] - 1))
                            {
                                _irc.SendChatMessage(Parser.Parse(Parser.Generations["Vote Succeed"].Next(), new KeyValuePair<string, string>("Name", item.Name)));
                                if(!string.IsNullOrEmpty(item.FileName))
                                    PlayAudio(Directory.GetCurrentDirectory() + @"\Audio\" + item.FileName);
                                Votes.Remove(item.Command);
                            }
                            else
                            {
                                Votes[item.Command] += 1;
                                _voteTimer.Stop();
                                _voteTimer.Start();
                                _irc.SendChatMessage(Parser.Parse(Parser.Generations["Vote"].Next(), new KeyValuePair<string, string>("Name", item.Name), new KeyValuePair<string, string>("Count", Votes[item.Command].ToString())));
                            }
                        }
                        else
                        {
                            Votes.Add(item.Command, 0);
                            _irc.SendChatMessage(Parser.Parse(Parser.Generations["New Vote"].Next(), new KeyValuePair<string, string>("Name", item.Name)));
                        }
                        break;
                    default:
                        Console.Write(
                            $"Unsupported {item.Action} action! You're doing something unsupported, unsupported I say!!");
                        break;
                }
                if (!(item.Cooldown > 0)) continue;
                var timer = new Timer(item.Cooldown*60000) {AutoReset = false, Enabled = true};
                timer.Elapsed += (sender, eventArgs) => { Frozen.Remove(item.Command); };
                Frozen.Add(item.Command);
            }
        }

        private static void MediaError(object pMediaObject)
        {
            MessageBox.Show("Cannot play media file. :~(");
        }

        private static void PlayAudio(string file)
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
