using Terminal.Gui;
using MultiTermCLI.Configuration;
using libCommunication.Configuration;

namespace MultiTermCLI.Tui;

public class MainTUIWindow : Window {
    private readonly Dictionary<string, TerminalPanel> _terminals;

    public MainTUIWindow() {
        Title = $"MultiTermCLI ({Application.QuitKey} to quit)";
        _terminals = [];
    }

    public MainTUIWindow(TerminalSettings settings) : this() {

        ColorScheme = new ColorScheme {
            Normal = new Terminal.Gui.Attribute(Color.Green, Color.Black),
            Focus = new Terminal.Gui.Attribute(Color.BrightGreen, Color.Black),
            HotNormal = new Terminal.Gui.Attribute(Color.Green, Color.Black),
            HotFocus = new Terminal.Gui.Attribute(Color.BrightGreen, Color.Black)
        };

        int count = settings.Terminals.Count;
        int cols = Math.Min(2, Math.Max(1, count >= 2 ? 2 : 1));
        int rows = (int)Math.Ceiling((double)count / cols);

        int index = 0;
        for (int r = 0; r < rows; r++) {
            for (int c = 0; c < cols && index < count; c++, index++) {
                SerialPortSettings t = settings.Terminals[index];

                FrameView frame = new() {
                    Title = t.PortName,
                    X = c == 0 ? 0 : Pos.Percent(50),
                    Y = r == 0 ? 0 : Pos.Percent(50),
                    Width = cols == 1 ? Dim.Fill() : Dim.Percent(50),
                    Height = rows == 1 ? Dim.Fill() : Dim.Percent(50)
                };

                TerminalPanel panel = new(t) {
                    X = 0,
                    Y = 0,
                    Width = Dim.Fill(),
                    Height = Dim.Fill()
                };

                _ = frame.Add(panel);
                _ = Add(frame);

                _terminals.Add(t.PortName, panel);
            }
        }

    }

}