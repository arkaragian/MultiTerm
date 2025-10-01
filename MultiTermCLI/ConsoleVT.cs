using System.Runtime.InteropServices;

internal static class ConsoleVT {
    private const int STD_OUTPUT_HANDLE = -11;
    private const int STD_INPUT_HANDLE = -10;

    private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004; // stdout
    private const uint DISABLE_NEWLINE_AUTO_RETURN = 0x0008; // optional
    private const uint ENABLE_VIRTUAL_TERMINAL_INPUT = 0x0200; // stdin (optional)


    private static IntPtr _hOut;
    private static IntPtr _hIn;
    private static uint _outModeOrig;
    private static uint _inModeOrig;
    private static bool _captured;

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

    public static bool IsVtEnabled() {
        if (!OperatingSystem.IsWindows()) {
            return true;
        }
        _hOut = GetStdHandle(STD_OUTPUT_HANDLE);
        if (_hOut == IntPtr.Zero || _hOut == new IntPtr(-1)) {
            return false;
        }
        if (!GetConsoleMode(_hOut, out uint mode)) {
            return false;
        }
        return (mode & ENABLE_VIRTUAL_TERMINAL_PROCESSING) != 0;
    }

    public static bool TryEnableVt() {
        if (!OperatingSystem.IsWindows()) {
            return true;
        }

        // Enable on stdout
        _hOut = GetStdHandle(STD_OUTPUT_HANDLE);

        if (_hOut == IntPtr.Zero || _hOut == new IntPtr(-1)) {
            return false;
        }
        if (!GetConsoleMode(_hOut, out _outModeOrig)) {
            return false; // ERROR_INVALID_HANDLE on MSYS2 pty
        }
        //uint newOutMode = outMode | ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN;
        uint newOutMode = _outModeOrig | ENABLE_VIRTUAL_TERMINAL_PROCESSING;
        if (!SetConsoleMode(_hOut, newOutMode)) {
            return false;
        }

        // Optional: enable VT input (for CSI key sequences)
        _hIn = GetStdHandle(STD_INPUT_HANDLE);
        if (_hIn != IntPtr.Zero && _hIn != new IntPtr(-1) && GetConsoleMode(_hIn, out _inModeOrig)) {
            SetConsoleMode(_hIn, _inModeOrig | ENABLE_VIRTUAL_TERMINAL_INPUT);
            //do not modify input
        }

        _captured = true;

        return true;
    }

    public static void Restore() {
        if (!OperatingSystem.IsWindows() || !_captured) { return; }
        if (_hOut != IntPtr.Zero && _hOut != new IntPtr(-1)) {
            SetConsoleMode(_hOut, _outModeOrig);
        }
        if (_hIn != IntPtr.Zero && _hIn != new IntPtr(-1)) {
            SetConsoleMode(_hIn, _inModeOrig);
        }
    }
}