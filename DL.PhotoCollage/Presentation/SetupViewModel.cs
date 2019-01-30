using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;

namespace DL.PhotoCollage.Presentation
{
    public class SetupViewModel
    {
        private readonly ApplicationController controller;

        private readonly Dictionary<BorderType, KeyValuePair<string, string>> borderTypePairs =
            new Dictionary<BorderType, KeyValuePair<string, string>>
            {
                { BorderType.None, new KeyValuePair<string, string>("none", "None") },
                { BorderType.Border, new KeyValuePair<string, string> ("border", "Border") },
                { BorderType.BorderHeader, new KeyValuePair<string, string> ("header", "Border with Header") },
                { BorderType.BorderFooter, new KeyValuePair<string, string> ("footer", "Border with Footer") }
            };

        private readonly Dictionary<ScreensaverSpeed, string> speedPairs =
            new Dictionary<ScreensaverSpeed, string>
            {
                { ScreensaverSpeed.Fast, "Fast" },
                { ScreensaverSpeed.Medium, "Medium" },
                { ScreensaverSpeed.Slow, "Slow" }
            };

        public SetupViewModel(ScreensaverConfiguration configurationToUse, ApplicationController controllerToUse)
        {
            this.BorderOptions = new ObservableCollection<KeyValuePair<string, string>>()
            {
               this.borderTypePairs[BorderType.None],
               this.borderTypePairs[BorderType.Border],
               this.borderTypePairs[BorderType.BorderHeader],
               this.borderTypePairs[BorderType.BorderFooter]
            };

            this.SpeedOptions = new ObservableCollection<string>()
            {
                this.speedPairs[ScreensaverSpeed.Slow],
                this.speedPairs[ScreensaverSpeed.Medium],
                this.speedPairs[ScreensaverSpeed.Fast]
            };

            this.Configuration = configurationToUse;
            this.controller = controllerToUse;
        }

        public ScreensaverConfiguration Configuration { get; private set; }

        public ObservableCollection<KeyValuePair<string, string>> BorderOptions { get; set; }

        public ObservableCollection<string> SpeedOptions { get; set; }

        public KeyValuePair<string, string> SelectedBorderType
        {
            get
            {
                return this.borderTypePairs[this.Configuration.PhotoBorderType];
            }
            set
            {
                var pair = this.borderTypePairs.Single(x => x.Value.Key == value.Key);
                this.Configuration.PhotoBorderType = pair.Key;
            }
        }

        public string SelectedSpeed
        {
            get
            {
                return this.speedPairs[this.Configuration.Speed];
            }
            set
            {
                var pair = this.speedPairs.Single(x => x.Value == value);
                this.Configuration.Speed = pair.Key;
            }
        }

        public void RequestDirectoryFromUser()
        {
            var dialog = new FolderBrowserDialog
            {
                Description = "Select a folder",
                ShowNewFolderButton = false
            };

            if (!string.IsNullOrWhiteSpace(this.Configuration.Directory))
            {
                dialog.SelectedPath = this.Configuration.Directory;
            }

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
    }
}
