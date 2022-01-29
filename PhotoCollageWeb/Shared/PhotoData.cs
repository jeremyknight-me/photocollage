namespace PhotoCollageWeb.Shared
{
    public class PhotoData
    {
        public PhotoData()
        {
            this.Id = Guid.NewGuid();
            this.IsRemoved = false;
        }

        public Guid Id { get; init; }
        public string Data { get; init; }
        public string Extension { get; init; }
        public string PositionStyles { get; set; }
        public bool IsRemoved { get; set; }
    }
}
