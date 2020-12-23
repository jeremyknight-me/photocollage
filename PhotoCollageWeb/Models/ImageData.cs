namespace PhotoCollageWeb.Models
{
    public record ImageData
    {
        public int Count { get; init; }
        public string Extension { get; init; }
        public string Data { get; init; }
    }
}
