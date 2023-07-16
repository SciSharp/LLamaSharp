using LLama.Common;

namespace LLama.Web.Models
{
    public class ModelOptions : ModelParams
    {
        public ModelOptions() : base("", 512, 20, 1337, true, true, false, false, "", "", -1, 512, false, false)
        {
        }

        public string Name { get; set; }
        public int MaxInstances { get; set; }

    }
}
