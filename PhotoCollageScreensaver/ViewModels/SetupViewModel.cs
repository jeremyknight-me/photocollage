using PhotoCollageScreensaver.Commands;
using PhotoCollageScreensaver.Enums;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Input;

namespace PhotoCollageScreensaver.ViewModels
{
    public class SetupViewModel : INotifyPropertyChanged
    {
        private readonly ApplicationController controller;

        private readonly Dictionary<BorderType, KeyValuePair<string, string>> borderTypePairs =
            new Dictionary<BorderType, KeyValuePair<string, string>>
            {
                { BorderType.None, new KeyValuePair<string, string>("none", BorderTypeNames.None) },
                { BorderType.Border, new KeyValuePair<string, string> ("border", BorderTypeNames.Border) },
                { BorderType.BorderHeader, new KeyValuePair<string, string> ("header", BorderTypeNames.BorderHeader) },
                { BorderType.BorderFooter, new KeyValuePair<string, string> ("footer", BorderTypeNames.BorderFooter) }
            };

        private readonly Dictionary<ScreensaverSpeed, string> speedPairs =
            new Dictionary<ScreensaverSpeed, string>
            {
                { ScreensaverSpeed.Fast, ScreensaverSpeedNames.Fast },
                { ScreensaverSpeed.Medium, ScreensaverSpeedNames.Medium },
                { ScreensaverSpeed.Slow, ScreensaverSpeedNames.Slow }
            };
        
        public SetupViewModel(Configuration config, ApplicationController controllerToUse)
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

            this.Config = config;
            this.controller = controllerToUse;

            this.PreviewCommand = new RelayCommand((obj) => this.controller.StartScreensaver());
            this.OkCommand = new RelayCommand(obj => {
                this.controller.SaveConfiguration();
                this.controller.Shutdown();
            });
            this.SaveCommand = new RelayCommand(obj => this.controller.SaveConfiguration());
            this.CancelCommand = new RelayCommand(obj => this.controller.Shutdown());
            this.SelectDirectoryCommand = new RelayCommand(obj =>
            {
                this.RequestDirectoryFromUser();
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand PreviewCommand { get; private set; }
        public ICommand OkCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        public ICommand SelectDirectoryCommand { get; private set; }

        public Configuration Config { get; private set; }
        public ObservableCollection<KeyValuePair<string, string>> BorderOptions { get; set; }
        public ObservableCollection<string> SpeedOptions { get; set; }

        public KeyValuePair<string, string> SelectedBorderType
        {
            get => this.borderTypePairs[this.Config.PhotoBorderType];
            set
            {
                var pair = this.borderTypePairs.Single(x => x.Value.Key == value.Key);
                this.Config.PhotoBorderType = pair.Key;
            }
        }

        public string SelectedDirectory
        {
            get => this.Config.Directory;
            set
            {
                if (value != this.Config.Directory)
                {
                    this.Config.Directory = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public double SelectedOpacity
        {
            get => this.Config.Opacity * 100.0;
            set => this.Config.Opacity = value / 100.0;
        }

        public string SelectedSpeed
        {
            get => this.speedPairs[this.Config.Speed];
            set
            {
                var pair = this.speedPairs.Single(x => x.Value == value);
                this.Config.Speed = pair.Key;
            }
        }

        private void RequestDirectoryFromUser()
        {
            var dialog = new FolderBrowserDialog
            {
                Description = "Select a folder",
                ShowNewFolderButton = false
            };
            if (!string.IsNullOrWhiteSpace(this.Config.Directory))
            {
                dialog.SelectedPath = this.Config.Directory;
            }

            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string path = dialog.SelectedPath;
                if (!string.IsNullOrEmpty(path))
                {
                    this.SelectedDirectory = path;
                }
            }
            else if (result != DialogResult.Cancel)
            {
                this.SelectedDirectory = string.Empty;
            }
        }

        public void Save()
        {
            this.controller.SaveConfiguration();
        }

        // This method is called by the Set accessor of each property.  
        // The CallerMemberName attribute that is applied to the optional propertyName  
        // parameter causes the property name of the caller to be substituted as an argument.  
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
