namespace LLama.Rag
{
    public class Rag
    {
        public static async Task Main(string[] args)
        {

            string startUrl = "https://en.wikipedia.org/wiki/Aluminium_alloy";
            //string startUrl = "https://google.com";
            int depth = 0;//Scrape only the webpage provided and no links. 
            int minWordLength = 10; //Specify the minimum number of words in a block that should be scraped.
            bool checkSentences = true;
            bool explodeParagraphs = true;

            WebScraper webScraper = await WebScraper.CreateAsync(startUrl, depth);

            var documentText = webScraper.ExtractVisibleText(minWordLength, checkSentences, explodeParagraphs);

            foreach (string text in documentText)
            {
                Console.WriteLine(text);
                Console.WriteLine("");
            }



        }
    }
}
