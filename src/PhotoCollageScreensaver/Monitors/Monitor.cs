namespace PhotoCollageScreensaver.Monitors;

/// <summary>This class deals with monitors.</summary>
internal static partial class Monitor
{
    private static List<Screen> _screens = null;

    internal static List<Screen> GetScreens()
    {
        _screens = [];
        var handler = new DisplayDevices.EnumMonitorsDelegate(MonitorEnumProc);
        DisplayDevices.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, handler, IntPtr.Zero); // should be sequential
        return _screens;
    }

    private static bool MonitorEnumProc(IntPtr hMonitor, IntPtr hdcMonitor, DisplayDevices.RECT rect, IntPtr dwData)
    {
        var mi = new DisplayDevices.MONITORINFO();
        if (DisplayDevices.GetMonitorInfo(hMonitor, mi))
        {
            var width = Math.Abs(mi.rcMonitor.Right - mi.rcMonitor.Left);
            var height = Math.Abs(mi.rcMonitor.Bottom - mi.rcMonitor.Top);
            var isPrimary = (mi.dwFlags & 1) == 1; // 1 = primary monitor
            var screen = new Screen(
                isPrimary,
                mi.rcMonitor.Top,
                mi.rcMonitor.Right,
                mi.rcMonitor.Bottom,
                mi.rcMonitor.Left,
                width,
                height
            );
            _screens.Add(screen);
        }

        return true;
    }
}
