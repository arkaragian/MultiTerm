using Terminal.Gui;
using MultiTermCLI.Configuration;
using libCommunication.Configuration;

namespace MuliTermCLI.Tui;

public class MainTUIWindow : Window {
    private Dictionary<string, TerminalPanel> _terminals;

    public MainTUIWindow(TerminalSettings settings) {
        foreach(SerialPortSettings t in settings.Terminals) {
            TerminalPanel panel = new(t);
            _terminals.Add(t.PortName, panel);
        }
    }

}