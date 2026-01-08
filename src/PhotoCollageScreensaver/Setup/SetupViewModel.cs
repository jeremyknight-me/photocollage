using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Input;
using PhotoCollageScreensaver.Collage.Presenters;
using PhotoCollageScreensaver.Commands;

namespace PhotoCollageScreensaver.Setup;

public class SetupViewModel : INotifyPropertyChanged
{
    private readonly ISettingsRepository _settingsRepo;
    private readonly IDictionary<BorderType, KeyValuePair<string, string>> _borderTypePairs = BorderTypeHelper.MakeDictionary();
    private readonly IDictionary<ScreensaverSpeed, string> _speedPairs = ScreensaverSpeedHelper.MakeDictionary();

    public SetupViewModel(
        CollagePresenterFactory collagePresenterFactory,
        ISettingsRepository settingsRepository)
    {
        _settingsRepo = settingsRepository;

        BorderOptions = new ObservableCollection<KeyValuePair<string, string>>(_borderTypePairs.Values);
        SpeedOptions = new ObservableCollection<string>(_speedPairs.Values);

        PreviewCommand = new RelayCommand((obj) => collagePresenterFactory.Create().Start());
        OkCommand = new RelayCommand(obj =>
        {
            _settingsRepo.Save();
            ShutdownHelper.Shutdown();
        });
        SaveCommand = new RelayCommand(obj => _settingsRepo.Save());
        CancelCommand = new ShutdownCommand();
        SelectDirectoryCommand = new RelayCommand(obj =>
        {
            RequestDirectoryFromUser();
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

    public KeyValuePair<string, string> SelectedBorderType
    {
        get => _borderTypePairs[Config.PhotoBorderType];
        set
        {
            KeyValuePair<BorderType, KeyValuePair<string, string>> pair = _borderTypePairs.Single(x => x.Value.Key == value.Key);
            Config.PhotoBorderType = pair.Key;
        }
    }

    public string SelectedDirectory
    {
        get => Config.Directory;
        set
        {
            if (value != Config.Directory)
            {
                Config.Directory = value;
                NotifyPropertyChanged();
            }
        }
    }

    public double SelectedOpacity
    {
        get => Config.Opacity * 100.0;
        set
        {
            Config.Opacity = value / 100.0;
            NotifyPropertyChanged();
        }
    }

    public string SelectedSpeed
    {
        get => _speedPairs[Config.Speed];
        set
        {
            KeyValuePair<ScreensaverSpeed, string> pair = _speedPairs.Single(x => x.Value == value);
            Config.Speed = pair.Key;
        }
    }

    public bool NumberOfPhotosEnabled => !Config.IsFullScreen;
    public bool MaximumPhotoSizeSliderEnabled => !Config.IsFullScreen;
    public bool BorderEnabled => !Config.IsFullScreen;

    public bool FullscreenMatchOrientationEnabled => Config.IsFullScreen;
    public bool RotateBasedOnEXIFEnabled => Config.IsFullScreen;

    public bool RotateBasedOnEXIFCheck
    {
        get => Config.RotateBasedOnEXIF;
        set
        {
            Config.RotateBasedOnEXIF = value;
            NotifyPropertyChanged();
        }
    }

    public bool FullScreenCheck
    {
        get => Config.IsFullScreen;
        set
        {
            Config.IsFullScreen = value;
            RotateBasedOnEXIFCheck = false;
            NotifyPropertyChanged();
            NotifyPropertyChanged(nameof(NumberOfPhotosEnabled));
            NotifyPropertyChanged(nameof(MaximumPhotoSizeSliderEnabled));
            NotifyPropertyChanged(nameof(BorderEnabled));
            NotifyPropertyChanged(nameof(RotateBasedOnEXIFCheck));
            NotifyPropertyChanged(nameof(RotateBasedOnEXIFEnabled));
            NotifyPropertyChanged(nameof(FullscreenMatchOrientationEnabled));
        }
    }

    public CollageSettings Config => _settingsRepo.Current;

    private void RequestDirectoryFromUser()
    {
        var dialog = new FolderBrowserDialog
        {
            ShowNewFolderButton = false
        };
        if (!string.IsNullOrWhiteSpace(Config.Directory))
        {
            dialog.SelectedPath = Config.Directory;
        }

        DialogResult result = dialog.ShowDialog();
        if (result == DialogResult.OK)
        {
            var path = dialog.SelectedPath;
            if (!string.IsNullOrEmpty(path))
            {
                SelectedDirectory = path;
            }
        }
        else if (result != DialogResult.Cancel)
        {
            SelectedDirectory = string.Empty;
        }
    }

    public void Save() => _settingsRepo.Save();

    // This method is called by the Set accessor of each property.  
    // The CallerMemberName attribute that is applied to the optional propertyName  
    // parameter causes the property name of the caller to be substituted as an argument.  
    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
