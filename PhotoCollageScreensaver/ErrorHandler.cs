using PhotoCollageScreensaver.Logging;

namespace PhotoCollageScreensaver;

public sealed class ErrorHandler
{
    private readonly ILogger logger;
    private readonly ISettingsRepository settingsRepo;

    public ErrorHandler(ILogger logger, ISettingsRepository settingsRepository)
    {
        this.logger = logger;
        this.settingsRepo = settingsRepository;
    }

    public void HandleError(Exception exception, bool showMessage = false)
    {
        this.LogMessage(exception);
        if (showMessage)
        {
            this.DisplayErrorMessage(exception.Message);
        }
    }

    public void DisplayErrorMessage(string message) => _ = MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

    public void LogMessage(Exception exception)
    {
        if (this.settingsRepo.Current.UseVerboseLogging)
        {
            this.logger.Log(exception.Message, exception.StackTrace);
        }
        else
        {
            this.logger.Log(exception.Message);
        }
    }
}
