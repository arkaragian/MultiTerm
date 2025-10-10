using System;
using System.Text;
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
    private readonly CheckBox _sendHEX;

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

    public TabBehavior? TabStop {
        get => _root.TabStop;
        set => _root.TabStop = value;
    }

    //public event Action<Key>? KeyDown;
    public event EventHandler<Key>? KeyDown;

    public TerminalInputLine() {
        _root = new View() {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = 1,
            CanFocus = true,
        };

        _input = new TextField() {
            X = 0,
            Y = 0,
            Width = Dim.Fill(19), // reserve room for " CR  LF HEX"
            Height = 1,
            ReadOnly = false,
            CanFocus = true,
            TabStop = TabBehavior.TabStop
        };

        _sendCR = new CheckBox() {
            Text = "CR",
            X = Pos.Right(_input) + 1,
            Y = 0,
            TabStop = TabBehavior.TabStop
        };

        _sendLF = new CheckBox() {
            Text = "LF",
            X = Pos.Right(_sendCR) + 1,
            Y = 0,
            TabStop = TabBehavior.TabStop
        };

        _sendHEX = new CheckBox() {
            Text = "HEX",
            X = Pos.Right(_sendLF) + 1,
            Y = 0,
            TabStop = TabBehavior.TabStop
        };

        _root.Add(_input);
        _root.Add(_sendCR);
        _root.Add(_sendLF);
        _root.Add(_sendHEX);

        // Capture key presses on the frame
        _input.KeyDown += (object? sender, Key e) => {
            KeyDown?.Invoke(sender, e);
        };
    }

    public byte[] BuildPayload() {

        if (_sendHEX.CheckedState is CheckState.Checked) {
            List<byte> result = new();

            string s = _input.Text?.ToString() ?? string.Empty;

            string[] string_bytes = s.Split(' ');
            foreach (string sb in string_bytes) {
                byte value = Convert.ToByte(sb.Substring(2), 16);
                result.Add(value);
            }

            if (_sendCR.CheckedState is CheckState.Checked) {
                result.Add((byte)'\r');
            }
            if (_sendLF.CheckedState is CheckState.Checked) {
                result.Add((byte)'\n');
            }

            _input.Text = "";
            return result.ToArray();

        } else {

            string s = _input.Text?.ToString() ?? string.Empty;

            if (_sendCR.CheckedState is CheckState.Checked) {
                s += (byte)'\r';
            }
            if (_sendLF.CheckedState is CheckState.Checked) {
                s += (byte)'\n';
            }

            _input.Text = "";
            return Encoding.ASCII.GetBytes(s);
        }

    }

    public void Dispose() {
        _sendLF?.Dispose();
        _sendCR?.Dispose();
        _input?.Dispose();
        _root?.Dispose();
    }
}