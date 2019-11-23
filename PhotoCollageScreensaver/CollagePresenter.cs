using PhotoCollageScreensaver.Contracts;
using PhotoCollageScreensaver.Repositories;
using PhotoCollageScreensaver.UserControls;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace PhotoCollageScreensaver
{
    public sealed class CollagePresenter
    {
        private readonly object threadLock = new object();
        private readonly Random random;
        private readonly List<ICollageView> views;
        private readonly IPhotoRepository photoRepository;
        private readonly ConcurrentQueue<CollageImage> imageQueue;
        private readonly ApplicationController controller;
        private int displayViewIndex;

        public CollagePresenter(ApplicationController controllerToUse, Configuration configurationToUse)
        {
            this.random = new Random();
            this.views = new List<ICollageView>();
            this.imageQueue = new ConcurrentQueue<CollageImage>();
            this.controller = controllerToUse;
            this.Configuration = configurationToUse;
            this.photoRepository = new PhotoRepositoryFactory(this.Configuration).Make();
            this.displayViewIndex = -1;
        }

        public Configuration Configuration { get; }

        public void StartAnimation()
        {
            try
            {
                if (!this.photoRepository.HasPhotos)
                {
                    this.controller.DisplayErrorMessage("Folder does not contain any supported photos.");
                    this.controller.Shutdown();
                }

                this.DisplayImageTimerTick(null, null);

                var seconds = (int)this.Configuration.Speed;
                var timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, seconds) };
                timer.Tick += this.DisplayImageTimerTick;
                timer.Start();
            }
            catch (Exception ex)
            {
                this.controller.HandleError(ex);
            }
        }

        public void Close()
        {
            foreach (var view in this.views)
            {
                view.Close();
            }
        }

        public int GetRandomNumber(int min, int max)
        {
            int value;
            lock (this.threadLock)
            {
                value = this.random.Next(min, max);
            }

            return value;
        }

        public void HandleError(Exception ex, bool showMessage = false)
        {
            this.controller.HandleError(ex, showMessage);
        }

        public void SetupWindow<T>(T window, System.Windows.Forms.Screen screen) where T : Window, ICollageView
        {
            var backgroundBrush = new SolidColorBrush
            {
                Opacity = this.Configuration.Opacity,
                Color = Colors.Black
            };
            window.Background = backgroundBrush;
            System.Drawing.Rectangle windowLocation = screen.Bounds;
            window.Left = windowLocation.Left;
            window.Top = windowLocation.Top;
            window.Width = windowLocation.Width;
            window.Height = windowLocation.Height;
            window.Show();
            this.views.Add(window);
        }

        private void DisplayImageTimerTick(object sender, EventArgs e)
        {
            try
            {
                string path = this.photoRepository.NextPhotoFilePath;
                ICollageView view = this.GetNextDisplayView();
                var control = new CollageImage(path, this);
                view.ImageCanvas.Children.Add(control);
                this.imageQueue.Enqueue(control);

                if (this.imageQueue.Count > this.Configuration.NumberOfPhotos)
                {
                    this.RemoveImageFromQueue();
                }

                this.SetUserControlPosition(control, view);
            }
            catch (Exception ex)
            {
                this.controller.HandleError(ex);
                this.controller.Shutdown();
            }
        }

        private ICollageView GetNextDisplayView()
        {
            int nextIndex = this.displayViewIndex + 1;
            if (nextIndex >= this.views.Count)
            {
                nextIndex = 0;
            }

            this.displayViewIndex = nextIndex;
            return this.views[nextIndex];
        }

        private void RemoveImageFromQueue()
        {
            if (this.imageQueue.TryDequeue(out CollageImage control))
            {
                Action<CollageImage> action = this.RemoveImageFromPanel;
                control.FadeOutImage(action);
            }
        }

        private void RemoveImageFromPanel(CollageImage control)
        {
            try
            {
                foreach (var view in this.views)
                {
                    if (view.ImageCanvas.Children.Contains(control))
                    {
                        view.ImageCanvas.Children.Remove(control);
                        control.Dispose();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                this.controller.HandleError(ex);
            }
        }

        private void SetUserControlPosition(UIElement control, ICollageView view)
        {
            var positioner = new ImagePositioner(this, control, view);
            positioner.Position();
        }
    }
}
