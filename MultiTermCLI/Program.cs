using libCommunication.Configuration;
using MultiTermCLI.Configuration;
using System.IO.Ports;
using Terminal.Gui;
using MultiTermCLI.Tui;

namespace MultiTermCLI;

public class Program {
    public static int Main(string[] args) {

        if (!ConsoleVT.IsVtEnabled()) {
            if (!ConsoleVT.TryEnableVt()) {
                Console.WriteLine("No VT Support! Exiting...");
                return 0;
            }
        }



        TerminalSettings settings = new() {
            Terminals = [
                new SerialPortSettings() {
                    PortName = "COM1",
                    BaudRate = 9600,
                    Parity = Parity.None,
                    DataBits = 8,
                    StopBits = StopBits.One,
                    LogSettings = new() {
                        LogLevel = Microsoft.Extensions.Logging.LogLevel.Information,
                    }
                },
                new SerialPortSettings() {
                    PortName = "COM2",
                    BaudRate = 9600,
                    Parity = Parity.None,
                    DataBits = 8,
                    StopBits = StopBits.One,
                    LogSettings = new() {
                        LogLevel = Microsoft.Extensions.Logging.LogLevel.Information,
                    }
                },
            ]
        };



        Application.Init();                         // set raw mode, alt screen, colors
        try {
            // optional: make Ctrl+C quit
            Application.QuitKey = Key.C.WithCtrl;   // maps to Command.Quit
            // or also wire SIGINT:

            Console.CancelKeyPress += static (_, e) => {
                e.Cancel = true;
                Application.RequestStop();
            };

            Toplevel top = new();
            _ = top.Add(new MainTUIWindow(/*settings*/));
            Application.Run(top);
            top.Dispose();
        } finally {
            Application.Shutdown();                 // restore terminal
        }



        return 0;
    }
}