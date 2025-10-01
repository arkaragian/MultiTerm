using libCommunication.Configuration;
using MultiTermCLI.Configuration;
using Terminal.Gui;

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

        (int rows, int cols, bool threeLayout) = DecideLayout(count);

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

                _ = frame.Add(panel.View);
                _ = Add(frame);

                _terminals.Add(t.PortName, panel);
            }
        }

    }

    public static (int rows, int columns, bool threeLayout) DecideLayout(int count) {

        bool threeLayout = false;
        int rows = 0;
        int cols = 0;

        if (count <= 0) {
            return (rows, cols, threeLayout);
        }

        if (count == 1) {
            rows = 1;
            cols = 1;
            return (rows, cols, threeLayout);
        }

        if (count == 2) { // upper and lower halves
            rows = 2;
            cols = 1;
            return (rows, cols, threeLayout);
        }

        if (count == 3) { // top full width, bottom two columns
            rows = 2;
            cols = 2;
            threeLayout = true;
            return (rows, cols, threeLayout);
        }

        if (count == 4) { // 2x2 quadrants
            rows = 2;
            cols = 2;
            return (rows, cols, threeLayout);
        }

        // >=5: near-square grid, capped to reasonable columns
        int maxCols = 4; // tune to taste for your terminal width
        cols = Math.Min(maxCols, (int)Math.Ceiling(Math.Sqrt(count)));
        rows = (int)Math.Ceiling((double)count / cols);
        return (rows, cols, threeLayout);
    }

    protected override void Dispose(bool disposing) {
        if (disposing) {
            foreach (KeyValuePair<string, TerminalPanel> a in _terminals) {
                a.Value.Dispose();
            }
        }

        base.Dispose(disposing);
    }

}