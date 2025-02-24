using System.Collections.Generic;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace LLama.Rag
{
    public interface IWebScraper
    {
        HashSet<string> VisitedUrls { get; }
        List<HtmlDocument> Documents { get; }

        Task<List<string>> ExtractVisibleTextAsync(int minWordLength, bool checkSentences, bool explodeParagraphs);
        Task<List<string>> ExtractParagraphsAsync();
    }
}