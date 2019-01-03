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
            this.PhotoBorderType = BorderType.Border;
            //this.ShowPhotoDate = true;
            this.Speed = ScreensaverSpeed.Medium;
            this.UseVerboseLogging = false;
        }

        public string Directory { get; set; }

        public bool IsGrayscale { get; set; }

        public bool IsRandom { get; set; }

        public int MaximumSize { get; set; }

        public int NumberOfPhotos { get; set; }

        public BorderType PhotoBorderType { get; set; }

        public bool ShowPhotoBorder
        {
            get
            {
                switch (this.PhotoBorderType)
                {
                    case BorderType.None:
                        return false;
                    case BorderType.Border:
                    case BorderType.BorderHeader:
                    case BorderType.BorderFooter:
                    default:
                        return true;
                }
            }
            set
            {
                this.PhotoBorderType = value ? BorderType.Border : BorderType.None;
            }
        }

        //public bool ShowPhotoDate { get; set; }

        public ScreensaverSpeed Speed { get; set; }

        public bool UseVerboseLogging { get; set; }
    }
}
