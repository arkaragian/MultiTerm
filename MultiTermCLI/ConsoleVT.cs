using System.Runtime.InteropServices;
using System;
using System.Runtime.InteropServices;

internal static class ConsoleVT {
    private const int STD_OUTPUT_HANDLE = -11;
    private const int STD_INPUT_HANDLE = -10;

    private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004; // stdout
    private const uint DISABLE_NEWLINE_AUTO_RETURN = 0x0008; // optional
    private const uint ENABLE_VIRTUAL_TERMINAL_INPUT = 0x0200; // stdin (optional)

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
        IntPtr hOut = GetStdHandle(STD_OUTPUT_HANDLE);
        if (hOut == IntPtr.Zero || hOut == new IntPtr(-1)) {
            return false;
        }
        if (!GetConsoleMode(hOut, out uint mode)) {
            return false;
        }
        return (mode & ENABLE_VIRTUAL_TERMINAL_PROCESSING) != 0;
    }

    public static bool TryEnableVt() {
        if (!OperatingSystem.IsWindows()) {
            return true;
        }

        // Enable on stdout
        IntPtr hOut = GetStdHandle(STD_OUTPUT_HANDLE);
        if (hOut == IntPtr.Zero || hOut == new IntPtr(-1)) {
            return false;
        }
        if (!GetConsoleMode(hOut, out uint outMode)) {
            return false; // ERROR_INVALID_HANDLE on MSYS2 pty
        }
        uint newOutMode = outMode | ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN;
        if (!SetConsoleMode(hOut, newOutMode)) {
            return false;
        }

        // Optional: enable VT input (for CSI key sequences)
        IntPtr hIn = GetStdHandle(STD_INPUT_HANDLE);
        if (hIn != IntPtr.Zero && hIn != new IntPtr(-1) && GetConsoleMode(hIn, out uint inMode)) {
            SetConsoleMode(hIn, inMode | ENABLE_VIRTUAL_TERMINAL_INPUT);
        }

        return true;
    }
}
