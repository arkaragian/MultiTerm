using Terminal.Gui;
using MultiTermCLI.Configuration;

namespace MuliTermCLI.Tui;

public class MainTUIWindow : Window {
    private Dictionary<string, TerminalPanel> _terminals;

    public MainTUIWindow(TerminalSettings settings) {
        foreach(var t in settings.Terminals) {
        }
    }

}