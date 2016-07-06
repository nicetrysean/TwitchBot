using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Timers;

namespace TwitchBot
{
    internal class Chat
    {

        private struct Message
        {
            public string Sender;
            public string Command;
        }

        private readonly IrcClient _irc;
        private readonly ConfigurationReader.Command[] _commands;

        private static readonly Regex UsernameRegex = new Regex(@"^:([\w]+?)!");
        private static readonly Regex CommandRegex = new Regex(@"(?::)(![\w]+)");

        private static readonly Dictionary<string, int> Votes = new Dictionary<string, int>();
        private static readonly Dictionary<string, int> Frozen = new Dictionary<string, int>();

        private const int VoteRefreshTime = 240;
        private static int _resetVoteTime;

        public Chat(IrcClient irc, ConfigurationReader.Command[] commands)
        {
            _irc = irc;
            _commands = commands;

            var timer = new Timer(1000)
            {
                AutoReset = true,
                Enabled = true
            };

            timer.Elapsed += TimerUpdate;
        }

        public void Update()
        {
            Message message = FetchMessage();
            
            for (int index = 0; index < _commands.Length; index++)
            {
                var item = _commands[index];
                if (item.Trigger != message.Command) continue;

                if ((Frozen.ContainsKey(item.Trigger) && Frozen[item.Trigger] > 0) || (item.Random && (item.RandomValue = new Random().Next(0, 101)) >= 100 - item.Chance))
                {
                    InjectParserDefinitions(message, item);

                    if (!string.IsNullOrEmpty(item.MisfireText))
                    {
                        _irc.SendChatMessage(Parser.Parse(item.MisfireText));
                        continue;
                    }
                }

                InjectParserDefinitions(message, item);

                switch (item.Action)
                {
                    case "Text":
                        _irc.SendChatMessage(Parser.Parse(item.Text));
                        break;
                    case "Audio":
                        PlayAudio($@"{Directory.GetCurrentDirectory()}\Audio\{item.FileName}");
                        _irc.SendChatMessage(item.Text);
                        break;
                    case "Vote":
                        if (Votes.ContainsKey(item.Trigger))
                        {
                            if (item.VotesRequired <= Votes[item.Trigger] - 1)
                            {
                                _irc.SendChatMessage(Parser.Parse(Parser.UserDefinedGenerations["Vote Succeed"].Next()));
                                if (!string.IsNullOrEmpty(item.FileName))
                                    PlayAudio(Directory.GetCurrentDirectory() + @"\Audio\" + item.FileName);
                                Votes.Remove(item.Trigger);
                            }
                            else
                            {
                                Votes[item.Trigger] += 1;
                                _resetVoteTime = VoteRefreshTime;
                                _irc.SendChatMessage(Parser.Parse(Parser.UserDefinedGenerations["Vote"].Next()));
                            }
                        }
                        else
                        {
                            Votes.Add(item.Trigger, 0);
                            _irc.SendChatMessage(Parser.Parse(Parser.UserDefinedGenerations["New Vote"].Next()));
                        }
                        break;
                    default:
                        Console.Write(
                            $"Unsupported {item.Action} action! You're doing something unsupported, unsupported I say!!");
                        break;
                }

                if (!(item.Cooldown > 0)) continue;
                Frozen[item.Trigger] = (int)item.Cooldown * 60;
            }
        }

        private Message FetchMessage()
        {
            string senderUsername = string.Empty;
            string command = string.Empty;
            var message = _irc.ReadMessage();
            if(!string.IsNullOrEmpty(message))
            {
                Console.WriteLine(message);

                Match cap;
                if ((cap = UsernameRegex.Match(message)).Success)
                {
                    senderUsername = cap.Groups[1].Value;
                }

                if ((cap = CommandRegex.Match(message)).Success)
                {
                    command = cap.Groups[1].Value;
                }
            }

            return new Message { Sender = senderUsername, Command = command };
        }

        private void PlayAudio(string file)
        {
            Program.PlayAudio(file);
        }

        private static void TimerUpdate(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if(Frozen.Count > 0)
            {
                foreach (var seconds in Frozen)
                {
                    if (seconds.Value > 0)
                    {
                        Frozen[seconds.Key] = seconds.Value - 1;
                    }
                }
            }

            if (Votes.Count > 0 && _resetVoteTime-- <= 0)
            {
                _resetVoteTime = VoteRefreshTime;
                Votes.Clear();
            }
        }

        private static void InjectParserDefinitions(Message message, ConfigurationReader.Command command)
        {
            string cooldown = "0s";
            string count = "0";

            if (command.Cooldown > 0 && Frozen.ContainsKey(command.Trigger))
                cooldown = new TimeSpan(0, 0, 0, Frozen[command.Trigger]).ToString();

            if (Votes.ContainsKey(command.Name))
                count = Votes[command.Name].ToString();

            //Supposedly this was fixed, but not :~(
            //discuss.dev.twitch.tv/t/whispers-over-irc-username-lowercase-uppercase/5697
            Parser.ApplicationDefined["Username"] = FirstLetterToUpper(message.Sender);

            Parser.ApplicationDefined["Name"] = command.Name;
            Parser.ApplicationDefined["Random"] = command.RandomValue.ToString();
            Parser.ApplicationDefined["Cooldown"] = cooldown;
            Parser.ApplicationDefined["Count"] = count;
        }

        public static string FirstLetterToUpper(string str)
        {
            if (str == null)
                return null;

            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);

            return str.ToUpper();
        }
    }
}
