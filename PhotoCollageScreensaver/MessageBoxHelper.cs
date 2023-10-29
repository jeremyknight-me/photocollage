namespace PhotoCollageScreensaver;

internal static class MessageBoxHelper
{
    public static void DisplayError(string message)
        => _ = MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
}
