namespace PhotoCollageWeb.Shared;

public class PhotoData
{
    public PhotoData()
    {
        this.Id = Guid.NewGuid();
    }

    public Guid Id { get; init; }
    public string Data { get; init; }
    public string Extension { get; init; }
}
