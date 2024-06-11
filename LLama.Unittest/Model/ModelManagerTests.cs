using LLama.Common;
using LLama.Model;

namespace LLama.Unittest;

public class ModelManagerTests
{
    private readonly ModelManager TestableModelManager;

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

        var model = await TestableModelManager.LoadModel(modelToLoad.FilePath, null!);

        Assert.Single(TestableModelManager.GetLoadedModels());

        var isLoaded = TestableModelManager.TryGetLoadedModel(model.ModelName, out var cachedModel);
        Assert.True(isLoaded);

        // unload
        Assert.True(TestableModelManager.UnloadModel(model.ModelName));

        Assert.Throws<ObjectDisposedException>(() => {
            _ = model.CreateContext(new ModelParams(modelToLoad.FilePath));
        });
    }
}
