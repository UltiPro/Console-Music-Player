using System.Runtime.InteropServices;
using LoggerTool;

namespace ManageWindowTool;

class ManageWindow
{
    private const int MF_BYCOMMAND = 0x00000000;
    private const int SC_CLOSE = 0xF060;
    private const int SC_MAXIMIZE = 0xF030;
    private const int SC_SIZE = 0xF000;
    [DllImport("user32.dll")]
    private static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);
    [DllImport("user32.dll")]
    private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
    [DllImport("kernel32.dll", ExactSpelling = true)]
    private static extern IntPtr GetConsoleWindow();
    public static void Start()
    {
        try
        {
            IntPtr consoleWindow = GetConsoleWindow();
            IntPtr systemMenu = GetSystemMenu(consoleWindow, false);
            if (consoleWindow != IntPtr.Zero)
            {
                DeleteMenu(systemMenu, SC_CLOSE, MF_BYCOMMAND);
                DeleteMenu(systemMenu, SC_MAXIMIZE, MF_BYCOMMAND);
                DeleteMenu(systemMenu, SC_SIZE, MF_BYCOMMAND);
            }
        }
        catch (Exception e)
        {
            Logger.SaveLog(e.Message);
        }
    }
}