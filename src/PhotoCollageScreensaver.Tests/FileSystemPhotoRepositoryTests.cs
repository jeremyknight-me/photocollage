using PhotoCollageScreensaver.Photos.FileSystem;
using PhotoCollageScreensaver.Photos.InMemory;

namespace PhotoCollageScreensaver.Tests;

public sealed class FileSystemPhotoRepositoryTests : IDisposable
{
    private readonly TempDirectory _temp = new();

    public void Dispose() => _temp.Dispose();

    [Fact]
    public void LoadPhotoPaths_ExcludedFolder_SkipsItAndEverythingBelow()
    {
        var kept = _temp.CreateFile(@"Photos\a.jpg");
        var keptNested = _temp.CreateFile(@"Photos\2024\b.png");
        _temp.CreateFile(@"Photos\Screenshots\c.jpg");
        _temp.CreateFile(@"Photos\Screenshots\Old\d.jpg");

        List<string> paths = LoadPaths([_temp.Combine("Photos")], [_temp.Combine(@"Photos\Screenshots")]);

        Assert.Equal(Sorted(kept, keptNested), Sorted([.. paths]));
    }

    [Fact]
    public void LoadPhotoPaths_ExclusionWithDifferentCaseAndTrailingSeparator_IsApplied()
    {
        var kept = _temp.CreateFile(@"Photos\a.jpg");
        _temp.CreateFile(@"Photos\WhatsApp\b.jpg");

        List<string> paths = LoadPaths([_temp.Combine("Photos")], [_temp.Combine(@"PHOTOS\whatsapp\")]);

        Assert.Equal([kept], paths);
    }

    [Fact]
    public void LoadPhotoPaths_SeveralFolders_ReturnsPhotosFromEachInFolderOrder()
    {
        var first = _temp.CreateFile(@"Family\a.jpg");
        var second = _temp.CreateFile(@"Holidays\b.jpg");

        List<string> paths = LoadPaths([_temp.Combine("Family"), _temp.Combine("Holidays")], []);

        Assert.Equal([first, second], paths);
    }

    [Fact]
    public void LoadPhotoPaths_OverlappingFolders_ReturnsEachPhotoOnce()
    {
        var outer = _temp.CreateFile(@"Photos\a.jpg");
        var inner = _temp.CreateFile(@"Photos\2024\b.jpg");

        List<string> paths = LoadPaths([_temp.Combine("Photos"), _temp.Combine(@"Photos\2024")], []);

        Assert.Equal(Sorted(outer, inner), Sorted([.. paths]));
    }

    [Fact]
    public void LoadPhotoPaths_OneFolderMissing_UsesTheRest()
    {
        var kept = _temp.CreateFile(@"Photos\a.jpg");

        List<string> paths = LoadPaths([_temp.Combine("Unplugged"), _temp.Combine("Photos")], []);

        Assert.Equal([kept], paths);
    }

    [Fact]
    public void LoadPhotoPaths_AllFoldersMissing_Throws()
    {
        var repository = new FileSystemPhotoRepository(
            new InMemoryOrderedPhotoPathRepository(),
            new StubSettingsRepository(new CollageSettings { Directories = [_temp.Combine("Unplugged")] }));

        Assert.Throws<DirectoryNotFoundException>(repository.LoadPhotoPaths);
    }

    [Theory]
    [InlineData("a.jpg", true)]
    [InlineData("a.JPEG", true)]
    [InlineData("a.png", true)]
    [InlineData("a.gif", false)]
    [InlineData("a.txt", false)]
    public void LoadPhotoPaths_FileExtension_OnlyLoadsSupportedPhotos(string fileName, bool expected)
    {
        var file = _temp.CreateFile($@"Photos\{fileName}");

        List<string> paths = LoadPaths([_temp.Combine("Photos")], []);

        Assert.Equal(expected, paths.Contains(file));
    }

    private static List<string> LoadPaths(List<string> directories, List<string> excludedDirectories)
    {
        var pathRepository = new InMemoryOrderedPhotoPathRepository();
        var settings = new CollageSettings { Directories = directories, ExcludedDirectories = excludedDirectories };
        new FileSystemPhotoRepository(pathRepository, new StubSettingsRepository(settings)).LoadPhotoPaths();

        List<string> paths = [];
        string path;
        while ((path = pathRepository.GetNextPath()) != string.Empty && !paths.Contains(path))
        {
            paths.Add(path);
        }

        return paths;
    }

    private static string[] Sorted(params string[] paths) => [.. paths.Order(StringComparer.OrdinalIgnoreCase)];

    private sealed class StubSettingsRepository(CollageSettings settings) : ISettingsRepository
    {
        public CollageSettings Current => settings;

        public void Load()
        {
        }

        public void Save()
        {
        }
    }
}
