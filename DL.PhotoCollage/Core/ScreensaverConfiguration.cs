using System;

namespace DL.PhotoCollage.Core
{
    public class ScreensaverConfiguration
    {
        public ScreensaverConfiguration()
        {
            this.Directory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            this.IsGrayscale = false;
            this.IsRandom = true;
            this.MaximumSize = 500;
            this.NumberOfPhotos = 10;
            this.ShowPhotoBorder = true;
            this.ShowPhotoDate = true;
            this.Speed = ScreensaverSpeed.Medium;
            this.UseVerboseLogging = false;
        }

        public string Directory { get; set; }

        public bool IsGrayscale { get; set; }

        public bool IsRandom { get; set; }

        public int MaximumSize { get; set; }

        public int NumberOfPhotos { get; set; }

        public bool ShowPhotoBorder { get; set; }

        public bool ShowPhotoDate { get; set; }

        public ScreensaverSpeed Speed { get; set; }

        public bool UseVerboseLogging { get; set; }
    }
}
