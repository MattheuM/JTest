using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JudoTests
{
    class Program
    {

        static void Main(string[] args)
        {
            var path = "";
            do
            {
                Console.WriteLine("Please enter path to input file:");
                path = Console.ReadLine();
            } while (!IsValidFile(path));

            var text = File.ReadAllText(path);
            var analytics = new WordAnalytics(text);
            const string prompt = "Please specify which part to show by typing 1 or 2, or 3 to exit:";
            var isExit = false;
            do
            {
                Console.WriteLine(prompt);
                var choice = Console.ReadLine();
                var parseChoice = (choice ?? "").Trim();
                switch (parseChoice)
                {
                    case "1":
                        var wordCounts = analytics.WordOccurenceCount();
                        var orderedCount = wordCounts.OrderBy(x => x.Key);
                        Console.WriteLine("\n");
                        Console.WriteLine("Word - Count");
                        foreach (KeyValuePair<string, int> keyValuePair in orderedCount)
                        {
                            Console.WriteLine(keyValuePair.Key + "  " + keyValuePair.Value);
                        }
                        break;
                    case "2":
                        Console.Write("Part 2 \n");
                        var concats = analytics.SixLetterConcatenationCount().OrderBy(x => x);
                        var joinedList = string.Join(", ", concats);
                        Console.WriteLine(joinedList);
                        break;
                    case "3":
                        isExit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid selection");
                        break;
                }

            } while (!isExit);
        }

        private static bool IsValidFile(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;
            if (!File.Exists(path))
                return false;
            if (Path.GetExtension(path) != ".txt")
                return false;
            return true;
        }
    }

    public class WordAnalytics
    {
        private List<string> Words { get; set; }
        public WordAnalytics(string text)
        {
            Words = new List<string>();
            var lines = text.Split(new[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var words = line.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                var withoutPunctuation = words.Select(RemovePunctuationFromWord);
                var nonEmptyWords = withoutPunctuation.Where(x => !string.IsNullOrWhiteSpace(x)).Select(w => w.ToLower()).ToArray();
                this.Words.AddRange(nonEmptyWords);
            }
        }

        static string RemovePunctuationFromWord(string word)
        {
            var whiteListed = word.Where(char.IsLetterOrDigit).ToArray();

            word = whiteListed.Aggregate(String.Empty, (acc, i) =>
            {
                return acc += i;
            });
            return word;
        }
        public IEnumerable<KeyValuePair<string, int>> WordOccurenceCount()
        {
            var result = from w in Words
                         group w by w into g
                         select new KeyValuePair<string, int>(g.Key, g.Count());
            return result;
        }

        public IEnumerable<string> SixLetterConcatenationCount()
        {
            var wordsLessThanSixChars = Words.Where(x => x.Length < 6).ToList();
            var concatenatedWords = new List<string>();
            foreach (var word in wordsLessThanSixChars)
            {
                var wordLength = word.Length;
                var localword = word; //Resharper prompt: Access to foreach variable may have different behaviours in different versions of the compiler.
                var suitableConcantenations = wordsLessThanSixChars.Where(x => x.Length == (6 - wordLength) && !x.Equals(localword)); //Assumes concatenation must be product of two different words
                foreach (var possibleWord in suitableConcantenations.Select(x => localword + x))
                {
                    if (Words.Any(w => w == possibleWord))
                        concatenatedWords.Add(possibleWord);
                }
            }
            return concatenatedWords.Distinct().ToList(); //Assumes distinct list
        }
    }

}
