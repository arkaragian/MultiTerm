using System;
using Terminal.Gui;

public sealed class TerminalInputLine : IDisposable {
    private readonly View _root;
    public View View {
        get { return _root; }
    }

    private readonly TextField _input;
    public TextField Input {
        get { return _input; }
    }

    private readonly CheckBox _sendCR;
    private readonly CheckBox _sendLF;

    // public bool AppendCR {
    //     get { return _sendCR.Checked; }
    //     set { _sendCR.Checked = value; }
    // }
    //
    // public bool AppendLF {
    //     get { return _sendLF.Checked; }
    //     set { _sendLF.Checked = value; }
    // }

    public Pos X {
        get => _root.X;
        set => _root.X = value;
    }

    public Pos Y {
        get => _root.Y;
        set => _root.Y = value;
    }

    public Dim? Width {
        get => _root.Width;
        set => _root.Width = value;
    }

    public Dim? Height {
        get => _root.Height;
        set => _root.Height = value;
    }

    public string Title {
        get => _root.Title;
        set => _root.Title = value;
    }

    public string Text {
        get => _input.Text;
        set => _input.Text = value;
    }

    public LineStyle BorderStyle {
        get => _root.BorderStyle;
        set => _root.BorderStyle = value;
    }

    public ColorScheme? ColorScheme {
        get => _root.ColorScheme;
        set => _root.ColorScheme = value;
    }

    //public event Action<Key>? KeyDown;
    public event EventHandler<Key>? KeyDown;

    public TerminalInputLine() {
        _root = new View() {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = 1,
            CanFocus = true
        };

        _input = new TextField() {
            X = 0,
            Y = 0,
            Width = Dim.Fill(18), // reserve room for " CR  LF "
            Height = 1,
            ReadOnly = false,
            CanFocus = true
        };

        _sendCR = new CheckBox() {
            Text = "CR",
            X = Pos.Right(_input) + 1,
            Y = 0
        };

        _sendLF = new CheckBox() {
            Text = "LF",
            X = Pos.Right(_sendCR) + 2,
            Y = 0
        };

        _root.Add(_input);
        _root.Add(_sendCR);
        _root.Add(_sendLF);

        // Capture key presses on the frame
        _input.KeyDown += (object? sender, Key e) => {
            KeyDown?.Invoke(sender, e);
        };
    }

    public string BuildPayload() {
        string s = _input.Text?.ToString() ?? string.Empty;
        // if (AppendCR) {
        //     s += "\r";
        // }
        // if (AppendLF) {
        //     s += "\n";
        // }
        return s;
    }

    public void Dispose() {
        _sendLF?.Dispose();
        _sendCR?.Dispose();
        _input?.Dispose();
        _root?.Dispose();
    }
}