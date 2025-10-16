using libMultiTerm.Configuration;
using Terminal.Gui;

namespace MultiTermCLI.Tui.SettingsDialog;

public class SettingsDialog : Dialog {

    //private readonly View _frame;
    private readonly HexSettings _inputSettings;
    private readonly HexSettings _displaySettings;
    //private readonly HexSettings _binary_display_settings;

    public bool Accepted { get; private set; }
    public HexSettings ResultInputSettings { get; private set; }
    public HexSettings ResultDisplaySettings { get; private set; }

    private readonly HexSettingsView hex_input_settings;
    private readonly HexSettingsView hex_display_settings;

    public SettingsDialog(HexSettings inputSettings, HexSettings displaySettings) : base() {
        //Title = "Settings";
        Modal = true;
        ShadowStyle = ShadowStyle.None;
        _inputSettings = inputSettings;
        _displaySettings = displaySettings;


        ColorScheme = new ColorScheme {
            Normal = new Terminal.Gui.Attribute(Color.Green, Color.Black),
            Focus = new Terminal.Gui.Attribute(Color.BrightGreen, Color.Black),
            Disabled = new Terminal.Gui.Attribute(Color.Green, Color.Gray)
        };

        hex_input_settings = new HexSettingsView(_inputSettings) {
            Title = "Hex Input Settings"
        };
        hex_display_settings = new HexSettingsView(_displaySettings) {
            Title = "Hex Display Settings",
            Y = Pos.Bottom(hex_input_settings)
        };

        _ = Add(hex_input_settings);
        _ = Add(hex_display_settings);


        Button ok = new() {
            Text = "OK",
            // X = Pos.AnchorEnd(offset: 20),
            // Y = Pos.Bottom(display_settings),
            CanFocus = true,
            TabStop = TabBehavior.TabStop
        };

        ok.Accepting += (sender, args) => {
            CommitSelections();
            Accepted = true;
            Application.RequestStop(this);
        };

        Button cancel = new() {
            Text = "Cancel",
            // X = Pos.AnchorEnd(offset: 12),
            // Y = Pos.Bottom(display_settings),
            CanFocus = true,
            //IsDefault = true,
            TabStop = TabBehavior.TabStop
        };

        cancel.Accepting += (sender, args) => {
            Accepted = false;
            Application.RequestStop(this);
        };

        // _ = Add(ok);
        // _ = Add(cancel);
        //
        // Width = Dim.Percent(50);
        // Height = Dim.Percent(46);

        AddButton(ok);
        AddButton(cancel);


        ResultInputSettings = new HexSettings() {
            InputFormat = HexFormat.ZeroPrefixed,
            Seperator = HexSequenceSeperator.Space,
        };

        ResultDisplaySettings = new HexSettings() {
            InputFormat = HexFormat.ZeroPrefixed,
            Seperator = HexSequenceSeperator.Space,
        };


        KeyDown += (sender, e) => {
            if (e == Key.Esc || e == Key.Q) {
                Application.RequestStop(this);
                e.Handled = true;
            }
        };

    }

    private void CommitSelections() {
        ResultInputSettings = hex_input_settings.Settings.Clone();
        ResultDisplaySettings = hex_display_settings.Settings.Clone();
    }
}