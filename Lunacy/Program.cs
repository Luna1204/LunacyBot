using DSharpPlus;
using DSharpPlus.EventArgs;
using System.Reflection;
using System.Text;

namespace Lunacy
{
    class LunacyMain
    {
        static void Main(string[] args)
        {

            MainAsync().GetAwaiter().GetResult();
        }

        public static string Prefix;
        static async Task MainAsync()
        {
            var Discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = Environment.GetEnvironmentVariable("TOKEN"),
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged
            });
            Discord.MessageCreated += async (s, e) =>
            {
                if (e.Message.MentionedUsers.Contains(s.CurrentUser))
                {
                    Commands.commands.First(c => c.name == "-help").command.Invoke(new Commands(), new object[] { s, e });
                    return;
                }
                foreach((string name, MethodInfo command, string help, string[]? aliases) c in Commands.commands)
                {
                    if (c.name == e.Message.Content.Split(' ')[0].ToLower() || (c.aliases != null && c.aliases.Any(s => s == e.Message.Content.Split(' ')[0].ToLower())))
                    {
                        if (c.command == null) await e.Message.RespondAsync("Something went wrong.");
                        else c.command.Invoke(new Commands(), new object[] { s, e });
                    }
                }
            };
            await Discord.ConnectAsync();
            await Task.Delay(-1);
        }

    }
}
