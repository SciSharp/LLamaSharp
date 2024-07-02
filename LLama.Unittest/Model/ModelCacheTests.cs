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
    public async void LoadModel_DisposesOnUnload()
    {
        const string modelId = "llama-2-7b";
        var modelToLoad = _testRepo.GetAvailableModels()
            .First(f => f.ModelFileName.Contains(modelId));

        // Load success
        var model = await TestableModelManager.LoadModelAsync(modelToLoad, modelId);
        Assert.NotNull(model);
        Assert.Equal(1, TestableModelManager.ModelsCached());

        // Load with same Id throws
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await TestableModelManager.LoadModelAsync(modelToLoad, modelId);
            return;
        });
        Assert.Equal(1, TestableModelManager.ModelsCached());

        // unloaded and disposed
        Assert.True(TestableModelManager.UnloadModel(modelId));
        Assert.Throws<ObjectDisposedException>(() =>
        {
            _ = model.CreateContext(new ModelParams(modelToLoad.ModelFileUri));
        });
        Assert.Equal(0, TestableModelManager.ModelsCached());

        // already unloaded and disposed
        Assert.False(TestableModelManager.UnloadModel(modelId));
        Assert.Throws<ObjectDisposedException>(() =>
        {
            _ = model.CreateContext(new ModelParams(modelToLoad.ModelFileUri));
        });

        // Can be reloaded after unload
        model = await TestableModelManager.LoadModelAsync(modelToLoad, modelId);
        Assert.NotNull(model);
        Assert.Equal(1, TestableModelManager.ModelsCached());
        Assert.True(TestableModelManager.UnloadModel(modelId));
        Assert.Equal(0, TestableModelManager.ModelsCached());
    }

    [Fact]
    public async void TryCloneLoadedModel_ClonesAndCaches()
    {
        const string modelId = "llama-2-7b";
        var modelToLoad = _testRepo.GetAvailableModels()
            .First(f => f.ModelFileName.Contains(modelId));

        var model = await TestableModelManager.LoadModelAsync(modelToLoad, modelId);
        Assert.NotNull(model);
        Assert.Equal(1, TestableModelManager.ModelsCached());

        // clone it -- Ref 2
        const string cloneId = nameof(cloneId);
        var isCachedAndCloned = TestableModelManager.TryCloneLoadedModel(modelId, cloneId, out var cachedModel);
        Assert.True(isCachedAndCloned);
        Assert.NotNull(cachedModel);
        Assert.Equal(2, TestableModelManager.ModelsCached());

        cachedModel.Dispose(); //-- ref 1
        Assert.True(TestableModelManager.UnloadModel(modelId));
        Assert.Equal(1, TestableModelManager.ModelsCached());

        // unloaded and disposed` -- ref 2
        Assert.True(TestableModelManager.UnloadModel(cloneId));
        Assert.Equal(0, TestableModelManager.ModelsCached());

        Assert.False(TestableModelManager.UnloadModel(modelId));
        Assert.False(TestableModelManager.UnloadModel(cloneId));
        Assert.Throws<ObjectDisposedException>(() =>
        {
            _ = model.CreateContext(new ModelParams(modelToLoad.ModelFileUri));
        });
        Assert.Throws<ObjectDisposedException>(() =>
        {
            _ = cachedModel.CreateContext(new ModelParams(modelToLoad.ModelFileUri));
        });
    }

    [Fact]
    public async void TryCloneLoadedModel_SameId_Throws()
    {
        const string modelId = "llama-2-7b";
        var modelToLoad = _testRepo.GetAvailableModels()
            .First(f => f.ModelFileName.Contains(modelId));

        var model = await TestableModelManager.LoadModelAsync(modelToLoad, modelId);
        Assert.NotNull(model);
        Assert.Equal(1, TestableModelManager.ModelsCached());

        // Same Id clone fails
        Assert.Throws<ArgumentException>(() =>
        {
            TestableModelManager.TryCloneLoadedModel(modelId, modelId, out var cachedModel);
        });
        Assert.Equal(1, TestableModelManager.ModelsCached());

        // Unload and dispose
        Assert.True(TestableModelManager.UnloadModel(modelId));
        Assert.Equal(0, TestableModelManager.ModelsCached());
        Assert.False(TestableModelManager.UnloadModel(modelId));
        Assert.Throws<ObjectDisposedException>(() =>
        {
            _ = model.CreateContext(new ModelParams(modelToLoad.ModelFileUri));
        });
    }
}
