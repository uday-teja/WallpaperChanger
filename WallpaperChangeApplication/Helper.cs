using System;
using System.Runtime.InteropServices;

namespace WallpaperChangeApplication
{
    public static class Helper
    {
        internal sealed class Win32
        {
            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            internal static extern int SystemParametersInfo(
                int uAction,
                int uParam,
                String lpvParam,
                int fuWinIni);
        }
    }
}
