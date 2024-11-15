namespace PhotoCollageWeb.Client.Models
{
    public record PhotoSettings
    {
        public Guid Id { get; init; }
        public int Index { get; init; }
        public string Source { get; init; }
        public bool HasBorder { get; init; }
        public bool IsGrayscale { get; init; }
        public int MaximumRotation { get; init; }
        public int MaximumSize { get; init; }
    }
}
