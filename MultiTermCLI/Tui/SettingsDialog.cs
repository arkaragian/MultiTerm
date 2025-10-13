using MultiTermCLI.Configuration;
using Terminal.Gui;

namespace MultiTermCLI.Tui;

public class SettingsDialog : Dialog {

    //private readonly View _frame;
    private readonly HexSettings _inputSettings;
    //private readonly HexSettings _binary_display_settings;

    RadioGroup _formats;
    RadioGroup _seperators;

    public bool Accepted { get; private set; }
    public HexSettings ResultSettings { get; private set; }

    public SettingsDialog(HexSettings inputSettings) : base() {
        //Title = "Settings";
        Modal = true;
        DefaultShadow = ShadowStyle.None;
        _inputSettings = inputSettings;

        ColorScheme = new ColorScheme {
            Normal = new Terminal.Gui.Attribute(Color.Green, Color.Black),
            Focus = new Terminal.Gui.Attribute(Color.BrightGreen, Color.Black),
            Disabled = new Terminal.Gui.Attribute(Color.Green, Color.Gray)
        };

        View hex_input_settings = BuildHexSettingsTab();
        View display_settings = BuildDisplayFormatsView();

        display_settings.Y = Pos.Bottom(hex_input_settings);

        _ = Add(hex_input_settings);
        _ = Add(display_settings);


        Button ok = new() {
            Text = "OK",
            X = Pos.AnchorEnd(offset: 20),
            Y = Pos.AnchorEnd(offset: 10),
            CanFocus = true,
            TabStop = TabBehavior.TabStop
        };

        ok.Accepting += (object? sender, CommandEventArgs args) => {
            CommitSelections();
            Accepted = true;
            Application.RequestStop(this);
        };

        Button cancel = new() {
            Text = "Cancel",
            X = Pos.AnchorEnd(offset: 12),
            Y = Pos.AnchorEnd(offset: 10),
            CanFocus = true,
            //IsDefault = true,
            TabStop = TabBehavior.TabStop
        };

        cancel.Accepting += (object? sender, CommandEventArgs args) => {
            Accepted = false;
            Application.RequestStop(this);
        };

        _ = Add(ok);
        _ = Add(cancel);

        ResultSettings = new HexSettings() {
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

    private View BuildHexSettingsTab() {


        View _container = new() {
            Title = "Hex Input Settings",
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Percent(35),
            CanFocus = true,
            TabStop = TabBehavior.NoStop
        };

        View _format_container = BuildFormatsView();

        _ = _container.Add(_format_container);


        TextView _example = new() {
            Title = "Hex Input Example",
            X = 0,
            Y = Pos.Bottom(_format_container) + 1,
            Width = Dim.Fill(),
            Height = 1,
            ReadOnly = true,
            BorderStyle = LineStyle.Single,
            CanFocus = false,
            Multiline = false,
            Text = "Hello World",//Payload.BuildPayload("Hello World", asHex: true, _inputSettings, false, false)
            //TabStop = TabBehavior.TabStop
        };

        _ = _container.Add(_example);



        //display_settings.Add(_container);
        return _container;

    }

    private View BuildFormatsView() {
        View _format_container = new() {
            Title = "Input Setting",
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = 8,
            BorderStyle = LineStyle.Single,
            CanFocus = true,
        };

        Label _lbl = new() {
            Text = "Hex Input Format",
            X = 0,
            Y = 0
        };

        _formats = new() {
            RadioLabels = [
                "Zero Prefixed: 0xAB",
                "H Prefixed   : hAB",
                "Non Prefixed : AB",
                "Decimal      : 171",
            ],
            X = 0,
            Y = Pos.Bottom(_lbl) + 1,
            Width = Dim.Percent(50),
            CanFocus = true,
            TabStop = TabBehavior.TabStop
        };

        switch (_inputSettings.InputFormat) {
            case HexFormat.ZeroPrefixed:
                _formats.SelectedItem = 0;
                break;
            case HexFormat.HPrefixed:
                _formats.SelectedItem = 1;
                break;
            case HexFormat.NonPrefixed:
                _formats.SelectedItem = 2;
                break;
            case HexFormat.Decimal:
                _formats.SelectedItem = 3;
                break;
        }

        _ = _format_container.Add(_lbl);
        _ = _format_container.Add(_formats);


        Line vLine = new() {
            X = Pos.Right(_formats),
            Y = 0,
            Height = 8,
            Orientation = Orientation.Vertical
        };

        _ = _format_container.Add(vLine);

        View _seperator_container = BuildSeperatorView();
        _seperator_container.X = Pos.Right(vLine);

        _ = _format_container.Add(_seperator_container);

        return _format_container;
    }

    private View BuildSeperatorView() {

        View _seperator_container = new() {
            X = 0,
            Y = 0,
            //Y = Pos.Bottom(_format_container),
            Width = Dim.Fill(),
            Height = 6,
            CanFocus = true,
        };

        Label _hex_seperator = new() {
            Text = "Hex Seperator:",
            X = 0,
            Y = 0,
        };

        _seperators = new() {
            RadioLabels = [
                "Space",
                "Comma"
            ],
            X = 0,
            Y = Pos.Bottom(_hex_seperator) + 1,
            CanFocus = true,
            TabStop = TabBehavior.TabStop
        };

        switch (_inputSettings.Seperator) {
            case HexSequenceSeperator.Space:
                _seperators.SelectedItem = 0;
                break;
            case HexSequenceSeperator.Comma:
                _seperators.SelectedItem = 1;
                break;
        }

        _ = _seperator_container.Add(_hex_seperator);
        _ = _seperator_container.Add(_seperators);

        return _seperator_container;
    }


    private View BuildDisplayFormatsView() {
        View _format_container = new() {
            Title = "Display Settings",
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            //Height = Dim.Percent(50),
            Height = 8,
            BorderStyle = LineStyle.Single,
            CanFocus = true,
        };

        Label _hex_format = new() {
            Text = "Hex Display:",
            X = 0,
            Y = 0,
        };

        RadioGroup _the_formats = new() {
            RadioLabels = [
                "ASCII            : Hello",
                "Zero Prefixed Hex: 0x48 0x65 0x6C 0x6C 0x6F",
                "H Prefixed Hex   : h48 h65 h6C h6C h6F",
                "Non Prefixed Hex : 48 65 6C 6C 6F",
                "Decimal          : 72 101 108 108 111",
            ],
            X = 0,
            Y = Pos.Bottom(_hex_format) + 1,
            CanFocus = true,
            TabStop = TabBehavior.TabStop
        };


        _ = _format_container.Add(_hex_format);
        _ = _format_container.Add(_the_formats);

        Line vLine = new() {
            X = Pos.Right(_the_formats),
            Y = 0,
            Height = 8,
            Orientation = Orientation.Vertical
        };

        RadioGroup _ascii_settings = new() {
            RadioLabels = [
                "Escape CRLF      : C",
            ],
            X = Pos.Right(vLine),
            Y = 0,
            CanFocus = true,
            TabStop = TabBehavior.TabStop
        };

        _ = _format_container.Add(vLine);
        _ = _format_container.Add(_ascii_settings);

        return _format_container;
    }

    private void CommitSelections() {
        ResultSettings = new HexSettings {
            InputFormat = _formats.SelectedItem switch {
                0 => HexFormat.ZeroPrefixed,
                1 => HexFormat.HPrefixed,
                2 => HexFormat.NonPrefixed,
                _ => HexFormat.Decimal
            },
            Seperator = _seperators.SelectedItem switch {
                0 => HexSequenceSeperator.Space,
                _ => HexSequenceSeperator.Comma
            }
        };
    }
}