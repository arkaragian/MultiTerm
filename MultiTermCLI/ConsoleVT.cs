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
            Console.WriteLine("Could not retreive the stdout handle");
            return false;
        }
        if (!GetConsoleMode(_hOut, out uint mode)) {
            Console.WriteLine("Could not stdout console mode");
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
            Console.WriteLine("Could not retreive the stdout handle");
            return false;
        }
        if (!GetConsoleMode(_hOut, out _outModeOrig)) {
            Console.WriteLine("Could not stdout console mode");
            return false; // ERROR_INVALID_HANDLE on MSYS2 pty
        }
        //uint newOutMode = outMode | ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN;
        uint newOutMode = _outModeOrig | ENABLE_VIRTUAL_TERMINAL_PROCESSING;
        if (!SetConsoleMode(_hOut, newOutMode)) {
            Console.WriteLine("Could not enable Virtual Terminal Processing");
            return false;
        }

        // Optional: enable VT input (for CSI key sequences)
        _hIn = GetStdHandle(STD_INPUT_HANDLE);
        if (_hIn != IntPtr.Zero && _hIn != new IntPtr(-1)) {
            bool ok = GetConsoleMode(_hIn, out _inModeOrig);
            if (ok) {
                ok = SetConsoleMode(_hIn, _inModeOrig | ENABLE_VIRTUAL_TERMINAL_INPUT);
                if (!ok) {
                    Console.WriteLine("Could not set Virtual Terminal Support for stdin");
                }
            }
            //do not modify input
        }

        _captured = true;

        return true;
    }

    public static void Restore() {
        if (!OperatingSystem.IsWindows() || !_captured) { return; }

        Console.WriteLine("Restoring Terminal");
        Console.Write("\u001b[?1049l\u001b[0m\u001b[?25h\r\n");

        if (_hOut != IntPtr.Zero && _hOut != new IntPtr(-1)) {
            bool ok = SetConsoleMode(_hOut, _outModeOrig);
            if (!ok) {
                Console.WriteLine("Could not restore stdout console mode");
            }
        }
        if (_hIn != IntPtr.Zero && _hIn != new IntPtr(-1)) {
            bool ok = SetConsoleMode(_hIn, _inModeOrig);
            if (!ok) {
                Console.WriteLine("Could not restore stdin console mode");
            }
        }
        Console.WriteLine("Terminal Restored");
    }
}