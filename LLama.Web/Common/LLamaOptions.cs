namespace LLama.Web.Common
{
    public class LLamaOptions
    {
        public AppType AppType { get; set; }
        public ModelCacheType ModelCacheType { get; set; }
        public List<ModelOptions> Models { get; set; }

        public void Initialize()
        {
        }
    }
}
