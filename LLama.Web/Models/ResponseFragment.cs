namespace LLama.Web.Models
{
    public class ResponseFragment
    {
        public ResponseFragment(string id, string content = null, bool isFirst = false, bool isLast = false)
        {
            Id = id;
            IsLast = isLast;
            IsFirst = isFirst;
            Content = content;
        }

        public string Id { get; set; }
        public string Content { get; set; }
        public bool IsLast { get; set; }
        public bool IsFirst { get; set; }
    }
}
