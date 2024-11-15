namespace PhotoCollageScreensaver.Monitors;

/// <summary>Represents a display device on a single system.</summary>
public sealed class Screen
{
    /// <summary>Initializes a new instance of the Screen class. </summary>
    /// <param name="primary">A value indicating whether the display is the primary screen.</param>
    /// <param name="top">The display's top X value.</param>
    /// <param name="right">The display's right Y value.</param>
    /// <param name="bottom">The display's bottom X value.</param>
    /// <param name="left">The display's left Y value.</param>
    /// <param name="width">The width of the display.</param>
    /// <param name="height">The height of the display.</param>
    internal Screen(bool primary, int top, int right, int bottom, int left, int width, int height)
    {
        this.IsPrimary = primary;
        this.Right = right;
        this.Left = left;
        this.Top = top;
        this.Bottom = bottom;
        this.Width = width;
        this.Height = height;
    }

    /// <summary>Gets a value indicating whether the display device is the primary monitor.</summary>
    internal bool IsPrimary { get; }

    /// <summary>Gets the display's leftmost X value.</summary>
    internal int Left { get; }
    /// <summary>Gets the display's rightmost X value.</summary>
    internal int Right { get; }

    /// <summary>Gets the display's top Y value.</summary>
    internal int Top { get; }

    /// <summary>Gets the display's bottom Y value.</summary>
    internal int Bottom { get; }

    /// <summary>Gets the width of the display.</summary>
    internal int Width { get; }

    /// <summary>Gets the height of the display.</summary>
    internal int Height { get; }
}
