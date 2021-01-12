using PhotoCollage.Common;
using PhotoCollageScreensaver.Contracts;
using PhotoCollageScreensaver.Repositories;
using PhotoCollageScreensaver.ViewModels;
using PhotoCollageScreensaver.Views;
using System;
using System.IO;
using System.Windows;

namespace PhotoCollageScreensaver
{
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
            var collagePresenter = this.CollagePresenter;
            foreach (var screen in System.Windows.Forms.Screen.AllScreens)
            {
                var collageWindow = new CollageWindow(this);
                collagePresenter.SetupWindow(collageWindow, screen);
            }

            collagePresenter.StartAnimation();
        }

        public void StartSetup() => new SetupWindow(this).Show();

        public CollagePresenter CollagePresenter => this.collagePresenter ?? (this.collagePresenter = new CollagePresenter(this, this.configuration));

        public void DisplayErrorMessage(string message) => _ = MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

        public void HandleError(Exception exception, bool showMessage = false)
        {
            this.LogErrorMessage(exception);
            if (showMessage)
            {
                this.DisplayErrorMessage(exception.Message);
            }
        }

        public void LogMessage(string message) => this.logger.Log(message);

        public SetupViewModel MakeSetupViewModel() => new SetupViewModel(this.configuration, this);

        public void Reset() => this.collagePresenter = null;

        public void SaveConfiguration() => this.configurationRepository.Save(this.configuration);

        public void Shutdown() => Application.Current.Shutdown();

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
