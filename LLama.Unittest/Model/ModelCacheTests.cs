using LLama.Common;
using LLama.Model;

namespace LLama.Unittest.Model;

public class ModelManagerTests
{
    private readonly ModelCache TestableModelManager;

    public ModelManagerTests()
    {
        TestableModelManager = new([Constants.ModelDirectory]);
    }

    [Fact]
    public void ModelDirectories_IsCorrect()
    {
        var dirs = TestableModelManager.ModelDirectories;
        Assert.Single(dirs);

        var expected = dirs.First()!.Contains(Constants.ModelDirectory);
        Assert.True(expected);
    }

    [Fact]
    public void AddDirectory_DoesntDuplicate()
    {
        for (var i = 0; i < 10; i++)
        {
            TestableModelManager.AddDirectory(Constants.ModelDirectory);
            TestableModelManager.AddDirectory(Path.GetFullPath(Constants.ModelDirectory));

            var dirs = TestableModelManager.ModelDirectories;
            Assert.Single(dirs);
            var expected = dirs.First()!.Contains(Constants.ModelDirectory);
            Assert.True(expected);
        }
    }

    [Fact]
    public void RemoveDirectory()
    {
        var dirs = TestableModelManager.ModelDirectories;
        Assert.Single(dirs);
        var expected = dirs.First()!.Contains(Constants.ModelDirectory);
        Assert.True(expected);

        Assert.True(TestableModelManager.RemoveDirectory(Constants.ModelDirectory));
        Assert.Empty(TestableModelManager.ModelDirectories);
        Assert.Empty(TestableModelManager.ModelFileList);
    }

    [Fact]
    public void RemoveDirectory_DoesNotExist()
    {
        var dirs = TestableModelManager.ModelDirectories;
        Assert.Single(dirs);
        var expected = dirs.First()!.Contains(Constants.ModelDirectory);
        Assert.True(expected);

        Assert.False(TestableModelManager.RemoveDirectory("foo/boo/bar"));
        Assert.Single(dirs);
    }

    [Fact]
    public void RemoveAllDirectories()
    {
        var dirs = TestableModelManager.ModelDirectories;
        Assert.Single(dirs);
        var expected = dirs.First()!.Contains(Constants.ModelDirectory);
        Assert.True(expected);

        TestableModelManager.RemoveAllDirectories();
        Assert.Empty(TestableModelManager.ModelDirectories);
        Assert.Empty(TestableModelManager.ModelFileList);
    }

    [Fact]
    public void ModelFiles_IsCorrect()
    {
        var files = TestableModelManager.ModelFileList;
        Assert.Equal(4, files.Count());
    }

    [Fact]
    public void GetAvailableModelsFromDirectory()
    {
        var files = TestableModelManager.GetAvailableModelsFromDirectory(Constants.ModelDirectory);
        Assert.Equal(4, files.Count());

        files = TestableModelManager.ModelFileList;
        Assert.Equal(4, files.Count());
    }

    [Fact]
    public void TryGetModelFileMetadata_WhenExists()
    {
        var expectedFile = TestableModelManager.ModelFileList.First();
        var found = TestableModelManager.TryGetModelFileMetadata(expectedFile.FilePath, out var foundData);

        Assert.True(found);
        Assert.Equal(expectedFile.FilePath, foundData.FilePath);
    }

    [Fact]
    public async void LoadModel_LoadsAndCaches()
    {
        var modelToLoad = TestableModelManager.ModelFileList
            .First(f => f.FileName.Contains("llama-2-7b"));

        var model = await TestableModelManager.LoadModel(modelToLoad.FilePath);
        var isLoaded = TestableModelManager.TryGetLoadedModel(model.ModelName, out var cachedModel);
        Assert.True(isLoaded);

        // unload the newly acquired model even though it was cached
        Assert.True(TestableModelManager.UnloadModel(model.ModelName));
        //cachedModel.Dispose(); // this does effectively nothing

        // unload "original"
        //model.Dispose();
        Assert.True(TestableModelManager.UnloadModel(model.ModelName));

        Assert.Throws<ObjectDisposedException>(() =>
        {
            _ = model.CreateContext(new ModelParams(modelToLoad.FilePath));
        });
    }

    [Fact]
    public async void LoadModel_AlreadyLoaded_ReturnsFromCache()
    {
        var modelToLoad = TestableModelManager.ModelFileList
            .First(f => f.FileName.Contains("llama-2-7b"));

        for (var i = 0; i < 5; i++)
        {
            var model = await TestableModelManager.LoadModel(modelToLoad.FilePath);
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
        var modelToLoad = TestableModelManager.ModelFileList
            .First(f => f.FileName.Contains("llama-2-7b"));

        using (var model = await TestableModelManager.LoadModel(modelToLoad.FilePath))
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
