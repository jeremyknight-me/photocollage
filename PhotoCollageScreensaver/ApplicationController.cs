using PhotoCollageScreensaver.Logging;
using PhotoCollageScreensaver.Data;
using PhotoCollageScreensaver.ViewModels;
using PhotoCollageScreensaver.Views;
using System.IO;
using PhotoCollage.Common;

namespace PhotoCollageScreensaver;

public class ApplicationController
{
    private readonly CollageSettings configuration;
    private readonly ISettingsRepository configurationRepository;
    private readonly ILogger logger;
    private CollagePresenter collagePresenter;

    public ApplicationController()
    {
        var localDataDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            @"DigitalLagniappe\Screensavers\PhotoCollage");
        this.logger = new TextLogger(localDataDirectory);
        this.configurationRepository = new FileSystemSettingsRepository(localDataDirectory);
        this.configuration = this.configurationRepository.Load();
    }

    public void StartScreensaver()
    {
        var collagePresenter = this.configuration.IsFullScreen ? this.collagePresenter ??= new CollagePresenterFullscreen(this, this.configuration) : this.collagePresenter ??= new CollagePresenterCollage(this, this.configuration);
        foreach (var screen in Monitors.Monitor.GetScreens())
        {
            var collageWindow = new CollageWindow(this);
            collagePresenter.SetupWindow(collageWindow, screen);
        }

        collagePresenter.StartAnimation();
    }

    public void StartSetup() => new SetupWindow(this).Show();

    public SetupViewModel MakeSetupViewModel() => new SetupViewModel(this.configuration, this);

    public void SaveConfiguration() => this.configurationRepository.Save(this.configuration);

    public void Shutdown() => Application.Current.Shutdown();

    public void HandleError(Exception exception, bool showMessage = false)
    {
        this.LogMessage(exception);
        if (showMessage)
        {
            this.DisplayErrorMessage(exception.Message);
        }
    }

    public void DisplayErrorMessage(string message) => _ = MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

    private void LogMessage(Exception exception)
    {
        if (this.configuration.UseVerboseLogging)
        {
            this.logger.Log(exception.Message, exception.StackTrace);
        }
        else
        {
            this.logger.Log(exception.Message);
        }
    }
}
