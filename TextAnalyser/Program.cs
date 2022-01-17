using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TextAnalyser
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please, specify the path to the processed file");
            Console.WriteLine("Pay attention, the result file will be saved to the same directory!");

            string filePathRead = Console.ReadLine();

            string filePathWrite = Path.GetDirectoryName(filePathRead) + "\\result.txt";

            SortedDictionary<String, String> words = new SortedDictionary<String, String>();

            try
            {
                Stopwatch swReading = new Stopwatch();
                swReading.Start();

                ReadFileToDictionary(filePathRead, out words);

                swReading.Stop();
                Console.WriteLine("File was read in " + swReading.ElapsedMilliseconds / 1000 + " seconds");
            }
            catch (IOException e)
            { 
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            { 
               Console.WriteLine(e.Message);
            }

            if (words.Count > 0)
            {
                try
                {
                    Stopwatch swWriting = new Stopwatch();
                    swWriting.Start();

                    WriteDictionaryToFile(filePathWrite, words);

                    swWriting.Stop();
                    Console.WriteLine("File was written in " + swWriting.ElapsedMilliseconds / 1000 + " seconds");
                }
                catch (IOException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

        }

        static void ReadFileToDictionary(string filePathRead, out SortedDictionary<String, String> words)
        {
            words = new SortedDictionary<String, String>();

            using (var reader = new StreamReader(new BufferedStream(File.OpenRead(filePathRead), 1024 * 1024)))
            {
                Regex regex = new Regex(@"[0-9A-Za-z]+", RegexOptions.IgnoreCase);

                int counter = 1;
                string value = "";
                string separator = ", ";

                while (!reader.EndOfStream)
                {
                    string currentstring = reader.ReadLine().ToLower();

                    MatchCollection matches = regex.Matches(currentstring);

                    var groupedWords = matches.GroupBy(i => i.Value);

                    foreach (var word in groupedWords)
                    {
                        if (words.TryGetValue(word.Key, out value))
                        {
                            words[word.Key] = value + separator + counter;
                        }
                        else
                        {
                            words.Add(word.Key, counter.ToString());
                        }
                    }

                    counter++;
                }
            }
        }

        static void WriteDictionaryToFile(string filePathWrite, SortedDictionary<String, String> words)
        {
            using (var writer = new StreamWriter(new BufferedStream(File.OpenWrite(filePathWrite), 1024 * 1024)))
            {
                foreach (var entry in words)
                    writer.WriteLine($"{entry.Key} [{entry.Value}]");
            }
        }
    }      
}
