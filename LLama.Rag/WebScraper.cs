using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using System.Web;

namespace LLama.Rag
{
    class WebScraper : IWebScraper
    {
        private static readonly HttpClient httpClient = new HttpClient();
        public HashSet<string> VisitedUrls { get; } = new HashSet<string>();
        public List<HtmlDocument> Documents { get; } = new List<HtmlDocument>();

        private WebScraper() { }

        public static async Task<WebScraper> CreateAsync(string url, int queryDepth)
        {
            WebScraper instance = new WebScraper();
            await instance.FetchContentAsynch(url, queryDepth);
            return instance;
        }

        private async Task FetchContentAsynch(string url, int queryDepth)
        {
            if (queryDepth < 0 || VisitedUrls.Contains(url)) return;

            try
            {
                VisitedUrls.Add(url);
                string pageContent = await httpClient.GetStringAsync(url);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(pageContent);
                Documents.Add(doc);

                if (queryDepth > 0)
                {
                    var links = ExtractLinks(doc, url);
                    var tasks = links.Select(link => FetchContentAsynch(link, queryDepth - 1));
                    await Task.WhenAll(tasks);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error scraping {url}: {ex.Message}");
            }
        }

        private static List<string> ExtractLinks(HtmlDocument doc, string baseUrl)
        {
            return doc.DocumentNode
                .SelectNodes("//body//a[@href]")?
                .Select(node => node.GetAttributeValue("href", ""))
                .Where(href => !string.IsNullOrEmpty(href))
                .Select(href => NormalizeUrl(href, baseUrl))
                .Where(link => link != null)
                .Distinct()
                .ToList() ?? new List<string>();
        }

        private static string NormalizeUrl(string href, string baseUrl)
        {
            if (href.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                return href;

            if (href.StartsWith("/"))
                return new Uri(new Uri(baseUrl), href).ToString();

            return null;
        }

        public async Task<List<string>> ExtractVisibleTextAsync(int minWordLength, bool checkSentences, bool explodeParagraphs)
        {
            return await Task.Run(() =>
            {
                List<string> allDocumentText = new List<string>();
                foreach (HtmlDocument doc in Documents)
                {
                    var currentDocText = doc.DocumentNode
                        .SelectNodes("//body//*[not(ancestor::table) and not(self::script or self::style)] | //body//a[not(self::script or self::style)]")?
                        .Select(node =>
                        {
                            string cleanedText = HtmlEntity.DeEntitize(node.InnerText.Trim());
                            cleanedText = cleanedText.Replace("\t", " ");
                            cleanedText = Regex.Replace(cleanedText, @"\s+", " ");
                            return cleanedText;
                        })
                        .Where(text => !string.IsNullOrWhiteSpace(text) && text.Split(' ').Length >= minWordLength)
                        .ToList() ?? new List<string>();

                    allDocumentText.AddRange(currentDocText);
                }

                if (explodeParagraphs) allDocumentText = ExplodeParagraphs(allDocumentText, minWordLength);
                if (checkSentences) allDocumentText = RudimentarySentenceCheck(allDocumentText);
                return allDocumentText;
            });
        }

        public async Task<List<string>> ExtractParagraphsAsync()
        {
            return await Task.Run(() =>
            {
                List<string> paragraphs = new List<string>();
                foreach (HtmlDocument doc in Documents)
                {
                    var currentDocParagraph = doc.DocumentNode
                        .SelectNodes("//p//text()")?
                        .Select(node => HtmlEntity.DeEntitize(node.InnerText.Trim()))
                        .Where(text => !string.IsNullOrWhiteSpace(text))
                        .ToList() ?? new List<string>();

                    paragraphs.AddRange(currentDocParagraph);
                }
                return paragraphs;
            });
        }

        private static List<string> RudimentarySentenceCheck(List<string> sentences)
        {
            List<Regex> sentenceRules = new List<Regex>
            {
                new Regex(@"^[A-Za-z0-9]+[\w\s,;:'""-]*", RegexOptions.Compiled | RegexOptions.IgnoreCase),
                new Regex(@"[^\W]{2,}", RegexOptions.Compiled),
                new Regex(@"\b(\w*:?[/\w\d]+\.){2,}\d+\b", RegexOptions.Compiled)
            };

            return sentences.Where(sentence => sentenceRules.All(regex => regex.IsMatch(sentence))).ToList();
        }

        private static List<string> ExplodeParagraphs(List<string> paragraphs, int minWordLength)
        {
            return paragraphs
                .SelectMany(paragraph =>
                    Regex.Matches(paragraph, @"(?<!\w\.\w.)(?<![A-Z][a-z]\.)(?<=\s|^)([A-Z0-9][^.!?]*[.!?])")
                        .Cast<Match>()
                        .Select(m => m.Value.Trim()))
                .ToList();
        }
    }
}
