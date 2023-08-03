using LLama.Web.Common;

namespace LLama.Web.Services
{
    public interface IModelCacheService
    {
        Task<LLamaModel> Create(ModelOptions modelOptions);
        Task<LLamaModel> Get(string modelName);
        Task<bool> Remove(string modelName);
    }
}