using LLama.Common;
using LLama.Model;

namespace LLama.Unittest.Model;

public class ModelManagerTests
{
    private readonly IModelSourceRepo _testRepo = new FileSystemModelRepo([Constants.ModelDirectory]);
    
    private readonly ModelCache TestableModelManager;

    public ModelManagerTests()
    {
        TestableModelManager = new();
    }

    [Fact]
    public async void LoadModel_LoadsAndCaches()
    {
        var modelToLoad = _testRepo.GetAvailableModels()
            .First(f => f.ModelFileName.Contains("llama-2-7b"));

        var model = await TestableModelManager.LoadModelAsync(modelToLoad);
        var isLoaded = TestableModelManager.TryGetLoadedModel(model.ModelName, out var cachedModel);
        Assert.True(isLoaded);

        // unload the newly acquired model even though it was cached
        Assert.True(TestableModelManager.UnloadModel(model.ModelName));
        //cachedModel.Dispose(); // this does effectively nothing

        // unload "original"
        model.Dispose(); // need to explicitly dispose the model that the caller (us) owns
        Assert.True(TestableModelManager.UnloadModel(model.ModelName));

        Assert.False(TestableModelManager.UnloadModel(model.ModelName));

        Assert.Throws<ObjectDisposedException>(() =>
        {
            _ = model.CreateContext(new ModelParams(modelToLoad.ModelFileUri));
        });
    }

    [Fact]
    public async void LoadModel_AlreadyLoaded_ReturnsFromCache()
    {
        var modelToLoad = _testRepo.GetAvailableModels()
            .First(f => f.ModelFileName.Contains("llama-2-7b"));

        for (var i = 0; i < 5; i++)
        {
            var model = await TestableModelManager.LoadModelAsync(modelToLoad);
            Assert.NotNull(model);
            Assert.Equal("LLaMA v2", model.ModelName);
            var isLoaded = TestableModelManager.TryGetLoadedModel(model.ModelName, out var cachedModel);
            Assert.True(isLoaded);
            Assert.NotNull(cachedModel);
            Assert.Equal("LLaMA v2", cachedModel.ModelName);
        }
    }

    [Fact]
    public async void TryGetLoadedModel_AlreadyDisposed_ReturnsFalse()
    {
        var modelToLoad = _testRepo.GetAvailableModels()
            .First(f => f.ModelFileName.Contains("llama-2-7b"));

        using (var model = await TestableModelManager.LoadModelAsync(modelToLoad))
        {
            Assert.NotNull(model);
            Assert.Equal("LLaMA v2", model.ModelName);
            var isLoaded = TestableModelManager.TryGetLoadedModel(model.ModelName, out var cachedModel);
            Assert.True(isLoaded);
            Assert.NotNull(cachedModel);
            Assert.Equal("LLaMA v2", cachedModel.ModelName);

            // unload from the last check
            Assert.True(TestableModelManager.UnloadModel("LLaMA v2"));

        } // end scope, dispose is called on the model but since we have the model cache it should stick around until unloaded
        Assert.True(TestableModelManager.UnloadModel("LLaMA v2"));

        // Model is still loaded due to cache
        var isDisposedLoaded = TestableModelManager.TryGetLoadedModel("LLaMA v2", out var disposedModel);
        Assert.False(isDisposedLoaded);
        Assert.Null(disposedModel);
    }
}
