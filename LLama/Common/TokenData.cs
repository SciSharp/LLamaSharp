namespace LLama.Common
{
    public record TokenData(int Id)
    {
        public float Logit { get; set; }
        public float Probability { get; set; }
        public string Content { get; set; }
    }
}
