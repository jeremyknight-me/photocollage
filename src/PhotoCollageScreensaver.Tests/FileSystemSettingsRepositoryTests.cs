namespace PhotoCollageScreensaver.Tests;

public sealed class FileSystemSettingsRepositoryTests : IDisposable
{
    private const string SettingsFileName = "photo-collage.config";
    private readonly TempDirectory _temp = new();

    public void Dispose() => _temp.Dispose();

    [Fact]
    public void Current_LegacyDirectorySetting_MovesIntoDirectories()
    {
        File.WriteAllText(_temp.Combine(SettingsFileName), """
            {
              "Directory": "C:\\Users\\someone\\Pictures",
              "IsFullScreen": true,
              "Speed": 20,
              "PhotoFullScreenMode": 1
            }
            """);

        CollageSettings settings = new FileSystemSettingsRepository(_temp.Path).Current;

        Assert.Equal([@"C:\Users\someone\Pictures"], settings.Directories);
        Assert.Empty(settings.ExcludedDirectories);
        Assert.Null(settings.Directory);
        Assert.True(settings.IsFullScreen);
        Assert.Equal(ScreensaverSpeed.SuperSlow, settings.Speed);
        Assert.Single(Directory.GetFiles(_temp.Path)); // loaded cleanly, no invalid-file backup
    }

    [Fact]
    public void Save_AfterLegacyLoad_WritesDirectoriesAndDropsDirectory()
    {
        File.WriteAllText(_temp.Combine(SettingsFileName), """{ "Directory": "C:\\Photos" }""");
        var repository = new FileSystemSettingsRepository(_temp.Path);
        _ = repository.Current;

        repository.Save();

        var json = File.ReadAllText(_temp.Combine(SettingsFileName));
        Assert.Contains("\"Directories\"", json);
        Assert.DoesNotContain("\"Directory\"", json);
        Assert.Equal([@"C:\Photos"], new FileSystemSettingsRepository(_temp.Path).Current.Directories);
    }

    [Fact]
    public void Current_NoSettingsFile_DefaultsToMyPictures()
    {
        CollageSettings settings = new FileSystemSettingsRepository(_temp.Path).Current;

        Assert.Equal([Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)], settings.Directories);
    }

    [Fact]
    public void Current_DirectoriesAndExclusions_RoundTrip()
    {
        var repository = new FileSystemSettingsRepository(_temp.Path);
        repository.Current.Directories = [@"C:\Photos", @"D:\Family"];
        repository.Current.ExcludedDirectories = [@"C:\Photos\Screenshots"];

        repository.Save();
        CollageSettings reloaded = new FileSystemSettingsRepository(_temp.Path).Current;

        Assert.Equal([@"C:\Photos", @"D:\Family"], reloaded.Directories);
        Assert.Equal([@"C:\Photos\Screenshots"], reloaded.ExcludedDirectories);
    }
}
