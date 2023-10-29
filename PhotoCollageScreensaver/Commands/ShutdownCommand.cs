namespace PhotoCollageScreensaver.Commands;

public class ShutdownCommand : RelayCommand
{
    public ShutdownCommand()
        : base(obj => ShutdownHelper.Shutdown())
    {
    }
}
