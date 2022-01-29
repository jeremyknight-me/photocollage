namespace PhotoCollageWeb.Shared
{
    public class PhotoData
    {
        public PhotoData()
        {
            this.Id = Guid.NewGuid();
        }

        public Guid Id { get; private set; }
    }
}
