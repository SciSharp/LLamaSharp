using LLama.Model;

namespace LLama.Unittest.Model;

public class FileSystemModelRepoTests
{
    private readonly FileSystemModelRepo TestableRepo;

    public FileSystemModelRepoTests()
    {
        TestableRepo = new([Constants.ModelDirectory]);
    }

    [Fact]
    public void ModelDirectories_IsCorrect()
    {
        var dirs = TestableRepo.ListSources();
        Assert.Single(dirs);

        var expected = dirs.First()!.Contains(Constants.ModelDirectory);
        Assert.True(expected);
    }

    [Fact]
    public void AddDirectory_DoesntDuplicate()
    {
        for (var i = 0; i < 10; i++)
        {
            TestableRepo.AddSource(Constants.ModelDirectory);
            TestableRepo.AddSource(Path.GetFullPath(Constants.ModelDirectory));

            var dirs = TestableRepo.ListSources();
            Assert.Single(dirs);
            var expected = dirs.First()!.Contains(Constants.ModelDirectory);
            Assert.True(expected);
        }
    }

    [Fact]
    public void RemoveDirectory()
    {
        var dirs = TestableRepo.ListSources();
        Assert.Single(dirs);
        var expected = dirs.First()!.Contains(Constants.ModelDirectory);
        Assert.True(expected);

        Assert.True(TestableRepo.RemoveSource(Constants.ModelDirectory));
        Assert.Empty(TestableRepo.ListSources());
        Assert.Empty(TestableRepo.GetAvailableModels());
    }

    [Fact]
    public void RemoveDirectory_DoesNotExist()
    {
        var dirs = TestableRepo.ListSources();
        Assert.Single(dirs);
        var expected = dirs.First()!.Contains(Constants.ModelDirectory);
        Assert.True(expected);

        Assert.False(TestableRepo.RemoveSource("foo/boo/bar"));
        Assert.Single(dirs);
    }

    [Fact]
    public void RemoveAllDirectories()
    {
        var dirs = TestableRepo.ListSources();
        Assert.Single(dirs);
        var expected = dirs.First()!.Contains(Constants.ModelDirectory);
        Assert.True(expected);

        TestableRepo.RemoveAllSources();
        Assert.Empty(TestableRepo.ListSources());
        Assert.Empty(TestableRepo.GetAvailableModels());
    }

    [Fact]
    public void ModelFiles_IsCorrect()
    {
        var files = TestableRepo.GetAvailableModels();
        Assert.Equal(4, files.Count());
    }

    [Fact]
    public void GetAvailableModelsFromDirectory()
    {
        var files = TestableRepo.GetAvailableModelsFromSource(Constants.ModelDirectory);
        Assert.Equal(4, files.Count());

        files = TestableRepo.GetAvailableModels();
        Assert.Equal(4, files.Count());
    }

    [Fact]
    public void TryGetModelFileMetadata_WhenExists()
    {
        var expectedFile = TestableRepo.GetAvailableModels().First();
        var found = TestableRepo.TryGetModelFileMetadata(expectedFile.ModelFileUri, out var foundData);

        Assert.True(found);
        Assert.Equal(expectedFile.ModelFileUri, foundData.ModelFileUri);
    }

}
