using libCommunication.Configuration;
using MultiTermCLI.Configuration;
using System.IO.Ports;
using MuliTermCLI.Tui;
using Terminal.Gui;

public class Program {
    public static int Main(string[] args) {
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
                }
            ]
        };




        Application.Init();
        MainTUIWindow mainWindow = new(settings);// {
            //ColorScheme = new ColorScheme {
            //    Normal = Application.Driver.MakeAttribute(Color.Green, Color.Black),
            //    Focus = Application.Driver.MakeAttribute(Color.BrightGreen, Color.Black),
            //    HotNormal = Application.Driver.MakeAttribute(Color.Green, Color.Black),
            //    HotFocus = Application.Driver.MakeAttribute(Color.BrightGreen, Color.Black)
            //}
        //};

        Application.Top.Add(mainWindow);
        Application.Run();
        Application.Shutdown();

        return 0;
    }
}