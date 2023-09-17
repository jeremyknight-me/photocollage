using PhotoCollage.Common;
using PhotoCollageScreensaver.Views;

namespace PhotoCollageScreensaver;

public interface ICollagePresenter
{
    CollageSettings Configuration
    {
        get;
    }

    void StartAnimation();

    int GetRandomNumber(int min, int max);

    void HandleError(Exception ex, bool showMessage = false);

    void SetupWindow<T>(T window, Monitors.Screen screen) where T : Window, ICollageView;
}
