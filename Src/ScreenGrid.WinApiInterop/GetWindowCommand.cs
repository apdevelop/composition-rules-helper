﻿namespace ScreenGrid.WinApiInterop
{
    // http://pinvoke.net/default.aspx/user32/GetWindow.html

    enum GetWindowCommand : uint
    {
        GW_HWNDFIRST = 0,
        GW_HWNDLAST = 1,
        GW_HWNDNEXT = 2,
        GW_HWNDPREV = 3,
        GW_OWNER = 4,
        GW_CHILD = 5,
        GW_ENABLEDPOPUP = 6
    }
}
