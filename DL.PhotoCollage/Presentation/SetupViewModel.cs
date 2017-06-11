using System;
using System.Windows.Forms;
using DL.PhotoCollage.Core;

namespace DL.PhotoCollage.Presentation
{
    public class SetupViewModel
    {
        private readonly ApplicationController controller;

        public SetupViewModel(ScreensaverConfiguration configurationToUse, ApplicationController controllerToUse)
        {
            this.Configuration = configurationToUse;
            this.controller = controllerToUse;
        }

        public ScreensaverConfiguration Configuration { get; private set; }

        public void RequestDirectoryFromUser()
        {
            var dialog = new FolderBrowserDialog
            {
                Description = "Select a folder",
                ShowNewFolderButton = false
            };

            var result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                string path = dialog.SelectedPath;
                if (!string.IsNullOrEmpty(path))
                {
                    this.Configuration.Directory = path;
                }
            }
            else if (result != DialogResult.Cancel)
            {
                this.Configuration.Directory = string.Empty;
            }
        }

        public void Save()
        {
            this.controller.SaveConfiguration();
        }

        public void SetScreensaverSpeed(string value)
        {
            this.Configuration.Speed = this.GetScreensaverSpeed(value);
        }

        private ScreensaverSpeed GetScreensaverSpeed(string value)
        {
            string loweredValue = value.ToLower();

            switch (loweredValue)
            {
                case "slow":
                    return ScreensaverSpeed.Slow;
                case "medium":
                    return ScreensaverSpeed.Medium;
                case "fast":
                    return ScreensaverSpeed.Fast;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
