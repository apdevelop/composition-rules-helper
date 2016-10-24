namespace ScreenGrid.WinApiInterop
{
    // https://msdn.microsoft.com/ru-ru/library/windows/desktop/ms632600%28v=vs.85%29.aspx
    // http://www.pinvoke.net/default.aspx/user32.getwindowlong

    /// <summary>
    /// Window Styles
    /// </summary>
    enum WindowStyle : uint
    {
        WS_VISIBLE = 0x10000000,
        WS_MINIMIZE = 0x20000000,
        WS_MAXIMIZE = 0x01000000,
    }
}
