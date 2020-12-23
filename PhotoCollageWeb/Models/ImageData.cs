using System;

namespace PhotoCollageWeb.Models
{
    public class ImageData
    {
        public ImageData(int maxRotation)
        {
            var random = new Random();
            this.PositionTop = random.Next(0, 100);
            this.PositionLeft = random.Next(0, 100);
            this.Rotation = random.Next(-maxRotation, maxRotation);
        }

        public Guid Key { get; } = Guid.NewGuid();
        public int Count { get; init; }
        public string Data { get; init; }
        public string Extension { get; init; }
        public int PositionLeft { get; }
        public int PositionTop { get; }
        public bool Removed { get; set; } = false;
        public int Rotation { get; }
    }
}
