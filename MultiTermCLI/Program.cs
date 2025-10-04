using libCommunication.Configuration;
using MultiTermCLI.Configuration;
using MultiTermCLI.Tui;
using System.IO.Ports;
using Terminal.Gui;

namespace MultiTermCLI;

public class Program {
    public static int Main(string[] args) {

        // if (!ConsoleVT.IsVtEnabled()) {
        //     Console.WriteLine("No Virtual Terminal Processing. Enabling virtual terminal");
        //     if (!ConsoleVT.TryEnableVt()) {
        //         Console.WriteLine("No VT Support! Exiting...");
        //         return 0;
        //     }
        // }



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
                // new SerialPortSettings() {
                //     PortName = "COM21",
                //     BaudRate = 9600,
                //     Parity = Parity.None,
                //     DataBits = 8,
                //     StopBits = StopBits.One,
                //     LogSettings = new() {
                //         LogLevel = Microsoft.Extensions.Logging.LogLevel.Information,
                //     }
                // }
            ]
        };




        Application.Init();                         // set raw mode, alt screen, colors
        try {
            // optional: make Ctrl+C quit
            Console.TreatControlCAsInput = false;
            Application.QuitKey = Key.C.WithCtrl;   // maps to Command.Quit
            // or also wire SIGINT:


            Toplevel top = new();
            Console.CancelKeyPress += (_, e) => {
                e.Cancel = true;
                Application.RequestStop(top);
            };

            // Window w = new Terminal.Gui.Window { X = 0, Y = 0, Width = Dim.Fill(), Height = Dim.Fill() };
            // w.Add(new Label() { X = 2, Y = 1, Title = "Hello From Terminal" });
            // top.Add(w);


            _ = top.Add(new MainTUIWindow(settings));
            Application.Run(top);
            top.Dispose();
        } finally {
            Application.Shutdown();                 // restore terminal
            //ConsoleVT.Restore();
            // Safety reset for alt screen, attributes, and cursor visibility.
        }



        return 0;
    }
}