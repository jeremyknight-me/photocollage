using System;
using System.IO;
using System.Windows;

namespace DL.PhotoCollage.Presentation
{
    public class ApplicationController
    {
        private readonly Application application;

        private readonly ScreensaverConfiguration configuration;

        private readonly IConfigurationRepository configurationRepository;

        private readonly ILogger logger;

        private CollagePresenter collagePresenter;

        public ApplicationController(Application applicationToUse)
        {
            this.application = applicationToUse;
            string localDataDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                @"DigitalLagniappe\Screensavers\PhotoCollage");
            this.logger = new TextLogger(localDataDirectory);
            this.configurationRepository = new FileSystemConfigurationRepository(localDataDirectory);
            this.configuration = configurationRepository.Load();
        }

        public CollagePresenter CollagePresenter => this.collagePresenter ?? (this.collagePresenter = new CollagePresenter(this, this.configuration));

        public void DisplayErrorMessage(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void HandleError(Exception exception, bool showMessage = false)
        {
            this.LogErrorMessage(exception);
            if (showMessage)
            {
                this.DisplayErrorMessage(exception.Message);
            }
        }

        public void LogMessage(string message)
        {
            this.logger.Log(message);
        }

        public SetupViewModel MakeSetupViewModel()
        {
            return new SetupViewModel(this.configuration, this);
        }

        public void Reset()
        {
            this.collagePresenter = null;
        }

        public void SaveConfiguration()
        {
            this.configurationRepository.Save(this.configuration);
        }

        public void Shutdown()
        {
            this.application.Shutdown();
        }

        private void LogErrorMessage(Exception exception)
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
}
