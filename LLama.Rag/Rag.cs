using System;
using System.Threading.Tasks;

namespace LLama.Rag
{
    public class Rag
    {
        public static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("Initializing WebScraper...");

                string startUrl = "https://en.wikipedia.org/wiki/Aluminium_alloy";
                int depth = 0; // Scrape only the provided webpage and no links. 
                int minWordLength = 4; // Minimum word count for a text block to be extracted.
                bool checkSentences = false;
                bool explodeParagraphs = true;

                WebScraper webScraper = await WebScraper.CreateAsync(startUrl, depth);

                Console.WriteLine("WebScraper initialized successfully.");
                Console.WriteLine("Extracting visible text...");

                var documentText = webScraper.ExtractVisibleTextAsync(minWordLength, checkSentences, explodeParagraphs);

                Console.WriteLine($"Extracted {documentText.Result.Count} blocks of text.");

                if (documentText.Result.Count == 0)
                {
                    Console.WriteLine("Warning: No text was extracted. Try lowering minWordLength or changing extraction settings.");
                }

                foreach (string text in documentText.Result)
                {
                    Console.WriteLine("Extracted Block:");
                    Console.WriteLine(text);
                    Console.WriteLine(""); // Space between blocks for readability
                }

                Console.WriteLine("Scraping complete.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
