namespace LLama.Web.Models
{
    public class ResponseFragment
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public bool IsLast { get; set; }
        public bool IsFirst { get; set; }
        public bool IsCancelled { get; set; }
        public int Elapsed { get; set; }
    }
}
