using LLama.Web.Common;

namespace LLama.Web.Services
{
    public interface IModelCacheService
    {
        LLamaModel GetOrCreate(ModelOptions modelOptions);
        bool Remove(string modelName);
    }
}