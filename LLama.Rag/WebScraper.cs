using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using System.Web;
using System.Security.Policy;
using System.IO;

namespace LLama.Rag;

    class WebScraper
    {
        private static readonly HttpClient httpClient = new HttpClient();
        public HashSet<string> visitedUrls = new HashSet<string>();
        public List<HtmlDocument> documents = new List<HtmlDocument>();

        private WebScraper()
        {

        }

        /// <summary>
        /// Asyncronously creates a WebQuery and stores all information related to the query in documents list.
        /// </summary>
        /// <param name="url">Url of website to retrieve data from.</param>
        /// <param name="queryDepth">Number of link levels to evaluate. </param>
        /// <returns>Returns a WebQuery object</returns>
        public static async Task<WebScraper> CreateAsync(string url, int queryDepth)
        {
            WebScraper instance = new WebScraper();
            await instance.FetchContentAsynch(url,queryDepth);
            return instance;
        }

        private async Task FetchContentAsynch(string url, int queryDepth)
        {
            if (queryDepth < 0 || visitedUrls.Contains(url)) return;
    
            try
            {   
                visitedUrls.Add(url);
                string pageContent = await httpClient.GetStringAsync(url);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(pageContent);
                
                documents.Add(doc);


                // Extract links and follow them if depth allows
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
            var links = doc.DocumentNode
                .SelectNodes("//body//a[@href]")?
                .Select(node => node.GetAttributeValue("href", ""))
                .Where(href => !string.IsNullOrEmpty(href))
                .Select(href => NormalizeUrl(href, baseUrl))
                .Where(link => link != null)
                .Distinct()
                .ToList() ?? new List<string>();

        
            return links;
        }

        private static string NormalizeUrl(string href, string baseUrl)
        {
            if (href.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                return href;

            if (href.StartsWith("/"))
                return new Uri(new Uri(baseUrl), href).ToString();

            return null;
        }

    /// <summary>
    /// Extracts all visible text from a webpage.
    /// </summary>
    /// <param name="minWordLength">Specifies the minimum number of words to in a group to collect, anything less is discarded</param>
    /// <param name="checkSentences">Performs a rudimentary sentance check. Removes any text with strings of non-text or addresses</param>
    /// <returns>Returns a list of strings List<strings></returns>
    public List<string> ExtractVisibleText(int minWordLength, bool checkSentences, bool explodeParagraphs)
        {
            //Select all text from body node
            List<string> allDocumentText = new List<string>();
            foreach (HtmlDocument doc in documents)
            {
                var currentDocText = doc.DocumentNode
                    .SelectNodes("//body//*[not(ancestor::table) and not(self::script or self::style)] | //body//a[not(self::script or self::style)]")?
                    .Select(node =>
                    {
                        // Get text and clean it
                        string cleanedText = HtmlEntity.DeEntitize(node.InnerText.Trim());

                        // Remove newlines, tabs, and extra spaces
                        cleanedText = cleanedText.Replace("\t", " "); // Replace tabs with space
                        //cleanedText = cleanedText.Replace("\n", ""); // Replaces newlines nothing
                        cleanedText = Regex.Replace(cleanedText, @"\s+", " "); // Replace multiple spaces with one

                        // Apply additional regex (e.g., remove unwanted characters)
                        //cleanedText = Regex.Replace(cleanedText, @"[^a-zA-Z0-9\s]", ""); // Remove non-alphanumeric character
                        
                     
                        return cleanedText;
                    })
                    .Where(text => !string.IsNullOrWhiteSpace(text) && text.Split(' ').Length >= minWordLength)
                    .ToList() ?? new List<string>();

                allDocumentText.AddRange(currentDocText);
            }

            if (explodeParagraphs) allDocumentText = ExplodeParagraphs(allDocumentText, minWordLength);
            if (checkSentences) allDocumentText = RudimentarySentenceCheck(allDocumentText);
            return allDocumentText;
        }
        public List<string> ExtractParagraphs()
        {
            List<string> paragraphs = new List<string>();
            foreach (HtmlDocument doc in documents) {
                //Select all text from body node
                var currentDocParagraph = doc.DocumentNode
                    .SelectNodes("//p//text()")?
                    .Select(node => HtmlEntity.DeEntitize(node.InnerText.Trim()))
                    .Where(text => !string.IsNullOrWhiteSpace(text))
                    .ToList() ?? new List<string>();

                paragraphs.AddRange(currentDocParagraph);
            }

            return paragraphs;
        }

        private static List<string> RudimentarySentenceCheck(List<string> sentences)
        {
            // Define regex patterns within the method
            List<Regex> sentenceRules = new List<Regex>
        {
                new Regex(@"^[A-Za-z0-9]+[\w\s,;:'""-]*", RegexOptions.Compiled | RegexOptions.IgnoreCase), // Contains valid words, no gibberish
                new Regex(@"[^\W]{2,}", RegexOptions.Compiled), //Removes sequences of non-word characters
                new Regex(@"\b(\w*:?[/\w\d]+\.){2,}\d+\b", RegexOptions.Compiled) //Remove iP and other adresses 

            };

            List<string> cleanedSentences = new List<string>();

            foreach (string sentence in sentences)
            {
                if (sentenceRules.All(regex => regex.IsMatch(sentence))) // Apply all regex rules
                {
                    cleanedSentences.Add(sentence);
                }
            }
            return cleanedSentences;
        }

        private static List<string> ExplodeParagraphs(List<string> paragraphs, int minWordLength)
        {
            List<string> allSentences = new List<string>();
            foreach (string paragraph in paragraphs)
            {

                List<string> paragraphSentences = Regex.Matches(paragraph, @"(?<!\w\.\w.)(?<![A-Z][a-z]\.)(?<=\s|^)([A-Z0-9][^.!?]*[.!?])")
                                     .Cast<Match>()  // Convert MatchCollection to IEnumerable<Match>
                                     .Select(m => m.Value.Trim())  // Extract matched text
                                     .ToList();
                allSentences.AddRange(paragraphSentences);
            }
            return allSentences;
        }

    }
