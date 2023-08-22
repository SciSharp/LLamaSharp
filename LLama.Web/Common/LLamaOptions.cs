using LLama.Web.Services;

namespace LLama.Web.Common
{
    public class LLamaOptions
    {
        public ModelCacheType ModelCacheType { get; set; }
        public List<ModelOptions> Models { get; set; }

        public void Initialize()
        {
        }
    }
}
