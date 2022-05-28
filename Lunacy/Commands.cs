using DSharpPlus;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MineStatLib;

namespace Lunacy
{
    public class Commands
    {
        public static List<(string name, MethodInfo? command, string help, string[]? aliases)> commands = new List<(string name, MethodInfo? command, string help, string[]? aliases)>()
        {
            ("-help", GetCommand("Help"), "Displays useful info about commands. Parameters none", new[] { "-?" }),
            ("-random", GetCommand("Random"), @"Generates a random number from x to y with an optional seed of z. Parameters x y [z]", null),
            ("-rate", GetCommand("Rate"), @"Rates x from 0 to 5. Parameters x", null),
            ("-8ball", GetCommand("EightBall"), "Gives a random response to a given prompt", new[] { "-eightball", "-balls" }),
            ("-status", GetCommand("Status"), "Gets the status of a minecraft server with IP x and port y. Parameters x [y]", null),
            ("-wordle", GetCommand("StartWordle"), "Starts a wordle tied to your user ID", null),
            ("-wguess", GetCommand("GuessWordle"), "Guesses x on the wordle currently tied to your user id, Parameters x", new string[] { "-wg" }),
            ("-invite", GetCommand("Invite"), "Sends a link to invite the bot to your server", null),
            ("-stainsoftime", GetCommand("StainsOfTime"), "Memes, The DNA of the soul.", new[] { "-monsoonmgrr" })
        };
        public static MethodInfo? GetCommand(string command)
        {
            Type thisType = new Commands().GetType();
            MethodInfo? theMethod = thisType.GetMethod(command);
            return theMethod;
        } 
        public static async void Help(DiscordClient s, MessageCreateEventArgs e)
        {
            string send = "```";
            foreach((string name, MethodInfo command, string help, string[]? aliases) t in commands)
            {
                string alias = "";
                int i = 0;
                if(t.aliases != null)foreach(string st in t.aliases)
                    {
                        alias += st + (i != t.aliases.Count() -1 ? ", " : " ");
                        i = i + 1;
                    }
                
                if(alias != "")send = send + t.name + ", Aliases:( " + alias + ") : " + t.help + "\n";
                else send = send + t.name + " : " + t.help + "\n";
            }
            send = send + "```";
            await e.Message.RespondAsync(send);
        }
        public static async void Random(DiscordClient s, MessageCreateEventArgs e)
        {
            List<object> args = e.Message.Content.Split(' ').ToList<object>();
            args.RemoveAt(0);
            //if (args.Any(n => !Int32.TryParse((string)n, out int n1))) await e.Message.RespondAsync("Invalid parameters.");
            if (args.Count < 2) await e.Message.RespondAsync("Not enough parameters.");
            try
            {
                if (args.Count == 2)
                {
                    await e.Message.RespondAsync("Number is: " + new Random().Next(Int32.Parse((string)args[0]), Int32.Parse((string)args[1])));
                }
                else if (args.Count >= 3)
                {
                    await e.Message.RespondAsync("Number is: " + new Random(args[2].GetHashCode()).Next(Int32.Parse((string)args[0]), Int32.Parse((string)args[1])));
                }
            }
            catch
            {
                await e.Message.RespondAsync("Invalid parameters.");
            }
        }
        public static async void Rate(DiscordClient s, MessageCreateEventArgs e)
        {
            string arg = e.Message.Content.Replace("-rate ", "");
            if (!arg.Contains("@everyone") && !arg.Contains("@here") && e.Message.MentionedRoles.Count < 1)
            {
                float rating = new Random(arg.GetHashCode()).Next(0, 10) / 2f;
                string rate = "";
                string[] rateAsChar = new string[5] { "🖤", "🖤", "🖤", "🖤", "🖤" };

                for (int j = 0; j < (int)rating; j++) rateAsChar[j] = "❤️";
                if (rating - (int)rating == 0.5f) rateAsChar[(int)rating] = "💔";
                foreach (string c in rateAsChar) rate += c;
                await e.Message.RespondAsync($"I rate {arg} : {rate} ({rating} / 5)");
            }
            else
            {
                await e.Message.RespondAsync("I rate You 🖤🖤🖤🖤🖤 (0/5)");
            }
        }
        public static async void EightBall(DiscordClient s, MessageCreateEventArgs e)
        {
            List<string> responses = new List<string>() { "As I see it, yes.", "Ask again later.", "Better not tell you now.", "Cannot predict now.", "Concentrate and ask again.",
             "Don’t count on it.", "It is certain.", "It is decidedly so.", "Most likely.", "My reply is no.", "My sources say no.",
             "Outlook not so good.", "Outlook good.", "Reply hazy, try again.", "Signs point to yes.", "Very doubtful.", "Without a doubt.",
             "Yes.", "Yes – definitely.", "You may rely on it." };
            e.Message.RespondAsync(responses[new Random().Next(0, responses.Count)]);
        }
        public static async void StainsOfTime(DiscordClient s, MessageCreateEventArgs e)
        {
            e.Message.RespondAsync(@"https://www.youtube.com/watch?v=OuSSXOQ-1bI&ab_channel=Crimson");
        }
        public static async void Status(DiscordClient s, MessageCreateEventArgs e)
        {
            string[] args = e.Message.Content.Split(' ');
            int port = 25565;
            string ip = "";
            if (args.Length > 2)
            {
                if (args[2] != null && Int32.TryParse(args[2], out port)) { }
                else {
                    e.Message.RespondAsync("Something went wrong witht the port");
                    return;
                }
            }
            if (args.Length < 2)
            {
                e.Message.RespondAsync("Not enough arguments");
                return;
            }
            ip = args[1];
            MineStat ms = new MineStat(ip, (ushort)port);
            Console.WriteLine("Minecraft server status of {0} on port {1}:", ms.Address, ms.Port);
            if (ms.ServerUp)
            {
                e.Message.RespondAsync($"Server is online running version {ms.Version} with {ms.CurrentPlayers} out of {ms.MaximumPlayers} players."
                + $"\nMessage of the day: {ms.Motd}"
                + $"\nLatency: {ms.Latency}ms");
            }
            else
                e.Message.RespondAsync("Server is offline!");
        }
        public static Dictionary<string, Wordle> wordles = new Dictionary<string, Wordle>()
        {

        };
        public static async void StartWordle(DiscordClient s, MessageCreateEventArgs e)
        {
            if(!wordles.ContainsKey(e.Message.Author.Id.ToString())) wordles.Add(e.Message.Author.Id.ToString(), new Wordle(e.Message.RespondAsync("balls").Result));
        }
        public static async void GuessWordle(DiscordClient s, MessageCreateEventArgs e)
        {
            if (wordles.ContainsKey(e.Message.Author.Id.ToString()))
            {
                string result = wordles[e.Message.Author.Id.ToString()].CheckGuess(e.Message.Content.Replace("-wg ", "").Replace("-wordleguess ", ""));
                if (result == "Correct")
                {
                    e.Message.Channel.SendMessageAsync($"You win :star2:\nThe word was: {wordles[e.Message.Author.Id.ToString()].word}");
                    wordles.Remove(e.Message.Author.Id.ToString());
                }
                else if(result == "Failed")
                {
                    e.Message.Channel.SendMessageAsync($"You fail :skull:\nThe word was: {wordles[e.Message.Author.Id.ToString()].word}");
                    wordles.Remove(e.Message.Author.Id.ToString());
                }
                e.Message.DeleteAsync();
            }
            else
            {
                e.Message.RespondAsync("You do not have a currently active wordle");
            }
        }
        public static async void Invite(DiscordClient s, MessageCreateEventArgs e)
        {
            e.Message.RespondAsync(@"https://discord.com/oauth2/authorize?client_id=974753443082272818&scope=bot&permissions=8");
        }
    }
}
