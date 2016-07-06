using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;

namespace TwitchBot
{
    internal class ConfigurationReader
    {
        public class Credentials
        {
            public string Username { get; private set; } = "Username";
            public string Password { get; private set; } = "oauth:xxxx";
            public string Channel { get; private set; } = "#channel";
        }

        public class Command
        {
            public string Name { get; set; } = "Name";
            public string Trigger { get; set; } = "!command";
            public string Action { get; set; } = "Text";
            public string Text { get; set; } = "";
            public string MisfireText { get; set; } = "This command is currently cooling down";
            public int VotesRequired { get; set; } = 3;
            public string FileName { get; set; } = "audio.mp3";
            public double Cooldown { get; set; } = 1;
            public bool Random { get; set; } = false;
            public int Chance { get; set; } = 0;
            [YamlIgnore] public int RandomValue;
        }

        public class Configuration
        {
            public Credentials User { get; set; } = new Credentials();
            public Command[] Commands { get; set; } = { new Command() };

            public Dictionary<string, string[]> Messages { get; set; } = new Dictionary<string, string[]>()
            {
                {
                    "Say", new[] {"Things", "To", "#Say#"}
                }
            };
        }

        public Configuration Data;

        public ConfigurationReader()
        {
            var deserializer = new Deserializer();
            if (File.Exists("config.yaml"))
            {
                using (StreamReader r = new StreamReader("config.yaml"))
                {
                    deserializer.Deserialize<Configuration>(r);
                }
            }
            else
            {
                Data = new Configuration();
                using (StreamWriter r = new StreamWriter("config.yaml"))
                {
                    var serializer = new Serializer(SerializationOptions.EmitDefaults);
                    serializer.Serialize(r, Data);
                }
            }
        }
        
    }
}
