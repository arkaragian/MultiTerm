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

    //public event Action<Key>? KeyDown;
    public event EventHandler<Key>? KeyDown;

    public HexSettings InputSettings { get; set; }

    public TerminalInputLine(HexSettings settings) {
        InputSettings = settings;

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
            Width = Dim.Fill(19), // reserve room for " CR  LF HEX"
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
            Text = "HEX",
            X = Pos.Right(_sendLF) + 1,
            Y = 0,
            TabStop = TabBehavior.TabStop
        };

        _ = View.Add(Input);
        _ = View.Add(_sendCR);
        _ = View.Add(_sendLF);
        _ = View.Add(_sendHEX);

        // Capture key presses on the frame
        Input.KeyDown += (sender, e) => {
            KeyDown?.Invoke(sender, e);
        };
    }

    public byte[]? BuildPayload() {

        if (_sendHEX.CheckedState is CheckState.Checked) {
            List<byte> result = [];

            string? s = Input.Text?.ToString();
            if (s is null) {
                return null;
            }
            //TODO: Validation
            //
            char sep = InputSettings.Seperator switch {
                HexSequenceSeperator.Space => ' ',
                HexSequenceSeperator.Comma => ',',
                _ => throw new NotImplementedException(),
            };

            string regex = InputSettings.InputFormat switch {
                HexFormat.ZeroPrefixed => @"^0x[0-9A-Fa-f]{2}$",
                HexFormat.HPrefixed => @"^h[0-9A-Fa-f]{2}$",
                HexFormat.NonPrefixed => @"^[0-9A-Fa-f]{2}$",
                HexFormat.Decimal => @"^(?:25[0-5]|2[0-4][0-9]|1?[0-9]{1,2})$",
                _ => throw new NotImplementedException(),
            };

            Regex rgo = new(regex);

            string[] string_bytes = s.Split(sep);

            foreach (string sb in string_bytes) {
                bool ok = rgo.IsMatch(sb);
                if (!ok) {
                    return null;
                }

                byte value = InputSettings.InputFormat switch {
                    HexFormat.ZeroPrefixed => Convert.ToByte(sb[2..], 16),
                    HexFormat.HPrefixed => Convert.ToByte(sb[1..], 16),
                    HexFormat.NonPrefixed => Convert.ToByte(sb, 16),
                    HexFormat.Decimal => byte.Parse(sb, CultureInfo.InvariantCulture),
                    _ => throw new NotImplementedException(),
                };
                result.Add(value);
            }


            if (_sendCR.CheckedState is CheckState.Checked) {
                result.Add((byte)'\r');
            }
            if (_sendLF.CheckedState is CheckState.Checked) {
                result.Add((byte)'\n');
            }

            Input.Text = "";
            return [.. result];

        } else {

            string? s = Input.Text?.ToString();
            if (s is null) {
                return [];
            }

            if (_sendCR.CheckedState is CheckState.Checked) {
                s += (byte)'\r';
            }
            if (_sendLF.CheckedState is CheckState.Checked) {
                s += (byte)'\n';
            }

            Input.Text = "";
            return Encoding.ASCII.GetBytes(s);
        }

    }

    public void Dispose() {
        _sendLF?.Dispose();
        _sendCR?.Dispose();
        Input?.Dispose();
        View?.Dispose();
    }
}