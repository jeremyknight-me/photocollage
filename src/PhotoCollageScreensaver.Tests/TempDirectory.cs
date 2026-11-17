namespace PhotoCollageScreensaver.Tests;

internal sealed class TempDirectory : IDisposable
{
    public TempDirectory()
    {
        Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"photocollage-tests-{Guid.NewGuid():N}");
        Directory.CreateDirectory(Path);
    }

    public string Path { get; }

    public string Combine(string relativePath) => System.IO.Path.Combine(Path, relativePath);

    public string CreateFile(string relativePath)
    {
        var fullPath = Combine(relativePath);
        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fullPath)!);
        File.WriteAllText(fullPath, string.Empty);
        return fullPath;
    }

    public string CreateDirectory(string relativePath) => Directory.CreateDirectory(Combine(relativePath)).FullName;

    public void Dispose() => Directory.Delete(Path, recursive: true);
}
