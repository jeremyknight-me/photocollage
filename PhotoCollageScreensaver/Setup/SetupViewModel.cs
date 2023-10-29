using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Input;
using PhotoCollage.Common.Settings;
using PhotoCollageScreensaver.Collage.Presenters;
using PhotoCollageScreensaver.Commands;

namespace PhotoCollageScreensaver.Setup;

public class SetupViewModel : INotifyPropertyChanged
{
    private readonly CollagePresenter presenter;
    private readonly ISettingsRepository settingsRepo;
    private readonly IDictionary<BorderType, KeyValuePair<string, string>> borderTypePairs = BorderTypeHelper.MakeDictionary();
    private readonly IDictionary<ScreensaverSpeed, string> speedPairs = ScreensaverSpeedHelper.MakeDictionary();
    private readonly IDictionary<FullScreenMode, KeyValuePair<string, string>> fullscreenModePairs = FullScreenModeHelper.MakeDictionary();

    public SetupViewModel(
        CollagePresenter collagePresenter,
        ISettingsRepository settingsRepository)
    {
        this.presenter = collagePresenter;
        this.settingsRepo = settingsRepository;

        this.BorderOptions = new ObservableCollection<KeyValuePair<string, string>>(this.borderTypePairs.Values);
        this.SpeedOptions = new ObservableCollection<string>(this.speedPairs.Values);
        this.FullScreenModeOptions = new ObservableCollection<KeyValuePair<string, string>>(this.fullscreenModePairs.Values);

        this.PreviewCommand = new RelayCommand((obj) => this.presenter.Start());
        this.OkCommand = new RelayCommand(obj =>
        {
            this.settingsRepo.Save();
            ShutdownHelper.Shutdown();
        });
        this.SaveCommand = new RelayCommand(obj => this.settingsRepo.Save());
        this.CancelCommand = new ShutdownCommand();
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


    public ObservableCollection<KeyValuePair<string, string>> BorderOptions { get; set; }
    public ObservableCollection<string> SpeedOptions { get; set; }
    public ObservableCollection<KeyValuePair<string, string>> FullScreenModeOptions { get; set; }

    public KeyValuePair<string, string> SelectedBorderType
    {
        get => this.borderTypePairs[this.Config.PhotoBorderType];
        set
        {
            var pair = this.borderTypePairs.Single(x => x.Value.Key == value.Key);
            this.Config.PhotoBorderType = pair.Key;
        }
    }

    public KeyValuePair<string, string> SelectedFullScreenMode
    {
        get => this.fullscreenModePairs[this.Config.PhotoFullScreenMode];
        set
        {
            var pair = this.fullscreenModePairs.Single(x => x.Value.Key == value.Key);
            this.Config.PhotoFullScreenMode = pair.Key;
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
                this.NotifyPropertyChanged();
            }
        }
    }

    public double SelectedOpacity
    {
        get => this.Config.Opacity * 100.0;
        set
        {
            this.Config.Opacity = value / 100.0;
            this.NotifyPropertyChanged();
        }
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

    public bool NumberOfPhotosEnabled => !this.Config.IsFullScreen;
    public bool MaximumPhotoSizeSliderEnabled => !this.Config.IsFullScreen;

    public bool FullScreenModeComboBoxEnabled => this.Config.IsFullScreen;
    public bool RotateBasedOnEXIFEnabled => this.Config.IsFullScreen;

    public bool RotateBasedOnEXIFCheck
    {
        get => this.Config.RotateBasedOnEXIF;
        set
        {
            this.Config.RotateBasedOnEXIF = value;
            this.NotifyPropertyChanged();
        }
    }

    public bool FullScreenCheck
    {
        get => this.Config.IsFullScreen;
        set
        {
            this.Config.IsFullScreen = value;
            this.RotateBasedOnEXIFCheck = false;
            this.NotifyPropertyChanged();
            this.NotifyPropertyChanged(nameof(this.NumberOfPhotosEnabled));
            this.NotifyPropertyChanged(nameof(this.MaximumPhotoSizeSliderEnabled));
            this.NotifyPropertyChanged(nameof(this.RotateBasedOnEXIFCheck));
            this.NotifyPropertyChanged(nameof(this.RotateBasedOnEXIFEnabled));
            this.NotifyPropertyChanged(nameof(this.FullScreenModeComboBoxEnabled));
        }
    }

    protected CollageSettings Config => this.settingsRepo.Current;

    private void RequestDirectoryFromUser()
    {
        var dialog = new FolderBrowserDialog
        {
            ShowNewFolderButton = false
        };
        if (!string.IsNullOrWhiteSpace(this.Config.Directory))
        {
            dialog.SelectedPath = this.Config.Directory;
        }

        var result = dialog.ShowDialog();
        if (result == DialogResult.OK)
        {
            var path = dialog.SelectedPath;
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

    public void Save() => this.settingsRepo.Save();

    // This method is called by the Set accessor of each property.  
    // The CallerMemberName attribute that is applied to the optional propertyName  
    // parameter causes the property name of the caller to be substituted as an argument.  
    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
