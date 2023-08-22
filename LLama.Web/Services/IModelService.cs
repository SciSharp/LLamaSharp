using LLama.Web.Common;

namespace LLama.Web.Services
{
    public interface IModelService
    {
        Task<LLamaContext> CreateContext(string modelName, string key);
        Task<LLamaContext> GetContext(string modelName, string key);
        Task<LLamaWeights> GetModel(string modelName);
        Task LoadModels();
        Task<LLamaWeights> LoadModel(ModelOptions modelParams);
        Task<bool> UnloadModel(string modelName);
        Task UnloadModels();
        Task<bool> RemoveContext(string modelName, string key);
        Task<LLamaContext> GetOrCreateModelAndContext(string modelName, string key);
    }
}
