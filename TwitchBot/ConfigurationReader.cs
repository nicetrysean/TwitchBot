using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

namespace TwitchBot
{
    internal class ConfigurationReader
    {
        public class Credentials
        {
            public string Username = "username";
            public string Password = "oauth:xxxxx";
            public string Channel = "#channel";
        }

        public class Item
        {
            public string Name;
            public string Command;
            public string Action;
            public string Text;
            public string MisfireText;
            public int VotesRequired;
            public string FileName;
            public double Cooldown;
        }

        public List<Item> Items;
        public Credentials User;
        public Dictionary<string, string[]> Texts;

        public ConfigurationReader(string credentials, string items, string messages)
        {
            if (!File.Exists(credentials))
            {
                Console.WriteLine("No credentials found! Created credentials.json for you to fill in.");
                using (StreamWriter r = new StreamWriter(credentials))
                {
                    r.Write(JsonConvert.SerializeObject(new Credentials(), Formatting.Indented));
                }
                return;
            }

            if (!File.Exists(items))
            {
                Console.WriteLine("No Commands found! Created commands.json");
                Items = new List<Item>(1) {new Item {Action = "Text", Text = "This is a live demo!", Command = "!demo", Cooldown = 0.1, Name = "Demo Text"} };
                using (StreamWriter r = new StreamWriter(items))
                {
                    r.Write(JsonConvert.SerializeObject(Items, Formatting.Indented));
                }
            }

            if (!File.Exists(messages))
            {
                Console.WriteLine("No Messages found! Created messages.json");
                Texts = new Dictionary<string, string[]>(1) {{"Demo", new[] {"Write", "Stuff", "Here"}}};
                using (StreamWriter r = new StreamWriter(messages))
                {
                    r.Write(JsonConvert.SerializeObject(Texts, Formatting.Indented));
                }
            }

            LoadItems(items);
            LoadTextMessages(messages);
            LoadCredentials(credentials);
        }

        public void LoadItems(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("No commands! Make sure commands.json is in the directory.");
                return;
            }
            using (StreamReader r = new StreamReader(filePath))
            {
                var json = r.ReadToEnd();
                Items = JsonConvert.DeserializeObject<List<Item>>(json);
            }
        }

        public void LoadTextMessages(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("No messages! Make sure messages.json is in the directory.");
                return;
            }
            using (StreamReader r = new StreamReader(filePath))
            {
                var json = r.ReadToEnd();
                Texts = JsonConvert.DeserializeObject<Dictionary<string,string[]>>(json);
            }
        }

        public void LoadCredentials(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("No credentials! Make sure credentials.json is in the directory.");
                return;
            }
            using (StreamReader r = new StreamReader(filePath))
            {
                var json = r.ReadToEnd();
                User = JsonConvert.DeserializeObject<Credentials>(json);
            }
        }
    }
}
