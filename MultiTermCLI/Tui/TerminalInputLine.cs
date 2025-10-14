using MultiTermCLI.Configuration;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Terminal.Gui;

namespace MultiTermCLI.Tui;

public sealed class TerminalInputLine : IDisposable {
    public View View { get; }
    public TextField Input { get; }

    private readonly CheckBox _sendCR;
    private readonly CheckBox _sendLF;
    private readonly CheckBox _sendHEX;
    private readonly CheckBox _displayHex;

    public Pos X {
        get => View.X;
        set => View.X = value;
    }

    public Pos Y {
        get => View.Y;
        set => View.Y = value;
    }

    public Dim? Width {
        get => View.Width;
        set => View.Width = value;
    }

    public Dim? Height {
        get => View.Height;
        set => View.Height = value;
    }

    public string Title {
        get => View.Title;
        set => View.Title = value;
    }

    public string Text {
        get => Input.Text;
        set => Input.Text = value;
    }

    public LineStyle BorderStyle {
        get => View.BorderStyle;
        set => View.BorderStyle = value;
    }

    public ColorScheme? ColorScheme {
        get => View.ColorScheme;
        set => View.ColorScheme = value;
    }

    public TabBehavior? TabStop {
        get => View.TabStop;
        set => View.TabStop = value;
    }

    public bool DisplayHex => _displayHex.CheckedState == CheckState.Checked;

    //public event Action<Key>? KeyDown;
    public event EventHandler<Key>? KeyDown;

    public HexSettings InputSettings { get; set; }

    private List<string> _history;
    int _history_index = -1;

    public TerminalInputLine(HexSettings settings) {
        InputSettings = settings;

        _history = new();

        View = new View() {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = 1,
            CanFocus = true,
        };

        Input = new TextField() {
            X = 0,
            Y = 0,
            Width = Dim.Fill(margin: 35), // reserve room for " CR  LF HEX"
            Height = 1,
            ReadOnly = false,
            CanFocus = true,
            TabStop = TabBehavior.TabStop
        };

        _sendCR = new CheckBox() {
            Text = "CR",
            X = Pos.Right(Input) + 1,
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
            Text = "Send HEX",
            X = Pos.Right(_sendLF) + 1,
            Y = 0,
            TabStop = TabBehavior.TabStop
        };

        _displayHex = new CheckBox() {
            Text = "Display HEX",
            X = Pos.Right(_sendHEX) + 1,
            Y = 0,
            TabStop = TabBehavior.TabStop
        };

        _ = View.Add(Input);
        _ = View.Add(_sendCR);
        _ = View.Add(_sendLF);
        _ = View.Add(_sendHEX);
        _ = View.Add(_displayHex);

        // Capture key presses on the frame
        Input.KeyDown += (sender, e) => {
            KeyDown?.Invoke(sender, e);
        };


        Input.KeyDown += (object? sender, Key e) => {
            if (e == Key.CursorUp) {

                if (_history_index < 0) {
                    _history_index = 0;
                }

                if (_history.Count is 0) {
                    e.Handled = true;
                    return;
                }

                Input.Text = _history[_history_index];
                //We decrement the index after providing the completion
                _history_index--;

                e.Handled = true;
                return;
            }

            if (e == Key.CursorDown) {
                _history_index--;
                if (_history_index < 0) {
                    _history_index = 0;
                    e.Handled = true;
                }

                if (_history_index > _history.Count - 1) {
                    Input.Text = _history.Last();
                    _history_index = _history.Count - 1;
                } else {
                    Input.Text = _history[_history_index];
                }

                e.Handled = true;
                return;

            }
        };
    }

    public byte[]? BuildPayload() {

        string? s = Input.Text?.ToString();
        if (s is null) {
            return null;
        }

        if (s is "") {
            return null;
        }

        if (_history.Count is 0) {
            _history.Add(s);
            _history_index++;
        } else {
            string prev = _history.Last();
            bool areDifferent = !s.Equals(prev, StringComparison.Ordinal);

            if (areDifferent) {
                _history.Add(s);
                _history_index++;
            }
        }

        bool asHex = _sendHEX.CheckedState is CheckState.Checked;
        bool sendCR = _sendCR.CheckedState is CheckState.Checked;
        bool sendLF = _sendLF.CheckedState is CheckState.Checked;
        byte[] result = Payload.BuildPayload(s, asHex, InputSettings, sendCR, sendLF);

        Input.Text = "";

        return result;
    }

    public void Dispose() {
        _sendLF?.Dispose();
        _sendCR?.Dispose();
        Input?.Dispose();
        View?.Dispose();
    }
}