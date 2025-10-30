using libMultiTerm;
using libMultiTerm.Configuration;
using Terminal.Gui;

namespace MultiTermCLI.Tui;

public sealed class TerminalInputLine : View {
    public TextField Input { get; }
    private readonly CheckBox _sendCR;
    private readonly CheckBox _sendLF;
    // private readonly CheckBox _sendHEX;
    // private readonly CheckBox _displayHex;

    private readonly RadioGroup _sendStyle;
    private readonly RadioGroup _displayStyle;

    //public bool DisplayHex => _displayHex.CheckedState == CheckState.Checked;
    public bool SendHex => _sendStyle.SelectedItem is 0;
    public bool SendText => _sendStyle.SelectedItem is 1;

    public bool DisplayHex => _displayStyle.SelectedItem is 0;
    public bool DisplayText => _displayStyle.SelectedItem is 1;

    public HexSettings InputSettings { get; set; }

    private HistoryProvider<string> _history_provider;
    // private List<string> _history;
    // int _history_index = -1;

    public TerminalInputLine(HexSettings settings) {
        InputSettings = settings;

        _history_provider = new();
        //_history = new();

        X = 0;
        Y = 0;
        Width = Dim.Fill();
        Height = 4;
        CanFocus = true;

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
            X = Pos.Right(Input) + 1,
            Y = 1,
            TabStop = TabBehavior.TabStop
        };

        // _sendHEX = new CheckBox() {
        //     Text = "Send HEX",
        //     X = Pos.Right(_sendLF) + 1,
        //     Y = 0,
        //     TabStop = TabBehavior.TabStop
        // };
        //
        // _displayHex = new CheckBox() {
        //     Text = "Display HEX",
        //     X = Pos.Right(_sendHEX) + 1,
        //     Y = 0,
        //     TabStop = TabBehavior.TabStop
        // };

        _sendStyle = new RadioGroup() {
            RadioLabels = [
                "Send Hex",
                "Send Text"
            ],
            X = Pos.Right(_sendCR) + 2,
            Y = 0,
            CanFocus = true,
            TabStop = TabBehavior.TabStop
        };

        _displayStyle = new RadioGroup() {
            RadioLabels = [
                "Display Hex",
                "Display Text"
            ],
            X = Pos.Right(_sendStyle) + 2,
            Y = 0,
            CanFocus = true,
            TabStop = TabBehavior.TabStop
        };

        _ = Add(Input);
        _ = Add(_sendCR);
        _ = Add(_sendLF);
        // _ = Add(_sendHEX);
        // _ = Add(_displayHex);
        _ = Add(_sendStyle);
        _ = Add(_displayStyle);


        Input.KeyDown += (object? sender, Key e) => {
            if (e == Key.CursorUp) {
                string? r = _history_provider.Back();
                if (r is not null) {
                    Input.Text = r;
                }

                e.Handled = true;
                return;
            }

            if (e == Key.CursorDown) {

                string? r = _history_provider.Next();
                if (r is not null) {
                    Input.Text = r;
                }

                e.Handled = true;
                return;
            }
        };
    }

    /// <summary>
    /// Generates the payload bytes that need to sent over the line. During generation
    /// the input field will be cleared.
    /// </summary>
    public byte[]? BuildPayload() {

        string? s = Input.Text;//?.ToString();
        if (s is null) {
            return null;
        }

        if (s is "") {
            return null;
        }

        _history_provider.AddItem(s);

        //bool asHex = _sendHEX.CheckedState is CheckState.Checked;
        bool sendCR = _sendCR.CheckedState is CheckState.Checked;
        bool sendLF = _sendLF.CheckedState is CheckState.Checked;
        byte[]? result = Payload.BuildPayload(s, SendHex, InputSettings, sendCR, sendLF);

        if (result is null) {
            return null;
        }

        Input.Text = "";

        return result;
    }
}