using DSharpPlus.Entities;

namespace Lunacy
{
    public class Wordle
    {
        public int guesses;
        public string word;
        public List<string> guessedWords = new List<string>() { };
        public DiscordMessage messageToEdit;
        public Wordle(DiscordMessage d)
        {
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory());
            string[] lines = File.ReadAllLines(@"C:\Users\Luna\source\repos\Lunacy\Lunacy\Words.txt");
            word = lines[new Random().Next(0, lines.Count())];
            guesses = 0;
            messageToEdit = d;
            messageToEdit.ModifyAsync($"```" +
                "\n⬛⬛⬛⬛⬛" +
                "\n⬛⬛⬛⬛⬛" +
                "\n⬛⬛⬛⬛⬛" +
                "\n⬛⬛⬛⬛⬛" +
                "\n⬛⬛⬛⬛⬛" +
                "```");
        }
        List<string> lines = new List<string>()
        {
            "⬛⬛⬛⬛⬛","⬛⬛⬛⬛⬛","⬛⬛⬛⬛⬛","⬛⬛⬛⬛⬛","⬛⬛⬛⬛⬛"
        };
        public string CheckGuess(string s)
        {
            if (s.Length == 5)
            {
                List<char> guessChars = s.ToCharArray().ToList();
                List<char> wordChars = word.ToCharArray().ToList();
                string line = "";
                Dictionary<char, int> Check = new Dictionary<char, int>();
                for (int i = 0; i < 5; i++)
                {
                    char guessChar = guessChars[i];
                    if (guessChar == wordChars[i])
                    {
                        line += "🟩";
                        wordChars[wordChars.IndexOf(guessChar)] = '?';
                    }
                    else if (wordChars.Contains(guessChar))
                    {
                        line += "🟧";
                        wordChars[wordChars.IndexOf(guessChar)] = '?';
                    }
                    else
                    {
                        line += "⬛";
                    }
                }
                line += $": {s}";
                lines[guesses] = line;
                guesses++;
                guessedWords.Add(s);
                string final = "";
                foreach(string s1 in lines)
                {
                    final += s1 + "\n";
                }
                messageToEdit.ModifyAsync($"```\n" + final + "\n```");
                if (guesses == 5 && s != word)
                {
                    return "Failed";
                }
                if (s == word) return "Correct";
                else return "Incorrect";
            }
            else return "Invalid";
        }
    }
}