using MultiTermCLI.Configuration;
using Terminal.Gui;

namespace MultiTermCLI.Tui;

public class SettingsDialog : Dialog {

    private readonly TabView _tabView;
    private readonly HexSettings _inputSettings;

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
            HotNormal = new Terminal.Gui.Attribute(Color.Green, Color.Black),
            HotFocus = new Terminal.Gui.Attribute(Color.BrightGreen, Color.Black),
            Disabled = new Terminal.Gui.Attribute(Color.Green, Color.Gray)
        };

        _tabView = new() {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            CanFocus = true,
        };

        Tab hex_input_settings = BuildHexSettingsTab();
        Tab display_settings = BuildDisplaySettingsTab();

        _tabView.AddTab(hex_input_settings, true);
        _tabView.AddTab(display_settings, false);

        _ = Add(_tabView);

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

    private Tab BuildHexSettingsTab() {

        Tab display_settings = new() {
            DisplayText = "Hex Input Settings",
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            CanFocus = true
        };


        View _format_container = BuildFormatsView();
        View _seperator_container = BuildSeperatorView();
        _seperator_container.Y = Pos.Bottom(_format_container);

        View _container = new() {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            CanFocus = true,
            TabStop = TabBehavior.NoStop
        };

        _ = _container.Add(_format_container);
        _ = _container.Add(_seperator_container);

        TextView _example = new() {
            Title = "Hex Input Example",
            X = 0,
            Y = Pos.Bottom(_seperator_container),
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

        _ = _container.Add(ok);
        _ = _container.Add(cancel);


        display_settings.View = _container;

        //display_settings.Add(_container);
        return display_settings;

    }

    private Tab BuildDisplaySettingsTab() {

        Tab display_settings = new() {
            DisplayText = "Display Settings",
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            CanFocus = true
        };


        View _format_container = BuildDisplayFormatsView();

        View _container = new() {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            CanFocus = true,
            TabStop = TabBehavior.NoStop
        };

        _ = _container.Add(_format_container);

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

        _ = _container.Add(ok);
        _ = _container.Add(cancel);


        display_settings.View = _container;

        //display_settings.Add(_container);
        return display_settings;


    }

    private View BuildFormatsView() {
        View _format_container = new() {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            //Height = Dim.Percent(50),
            Height = 8,
            BorderStyle = LineStyle.Single,
            CanFocus = true,
        };

        Label _hex_format = new() {
            Text = "Hex Format:",
            X = 0,
            Y = 0,
        };

        _formats = new() {
            RadioLabels = [
                "Zero Prefixed: 0xAB",
                "H Prefixed   : hAB",
                "Non Prefixed : AB",
                "Decimal      : 171",
            ],
            X = 0,
            Y = Pos.Bottom(_hex_format) + 1,
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

        _ = _format_container.Add(_hex_format);
        _ = _format_container.Add(_formats);

        return _format_container;
    }

    private View BuildSeperatorView() {

        View _seperator_container = new() {
            X = 0,
            Y = 0,
            //Y = Pos.Bottom(_format_container),
            Width = Dim.Fill(),
            Height = 6,
            BorderStyle = LineStyle.Single,
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
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            //Height = Dim.Percent(50),
            Height = 8,
            BorderStyle = LineStyle.Single,
            CanFocus = true,
        };

        Label _hex_format = new() {
            Text = "Display Format:",
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

        RadioGroup _ascii_settings = new() {
            RadioLabels = [
                "Escape CRLF      : C",
            ],
            X = 0,
            Y = Pos.Bottom(_the_formats) + 1,
            CanFocus = true,
            TabStop = TabBehavior.TabStop
        };

        // switch (_inputSettings.InputFormat) {
        //     case HexInputFormat.ZeroPrefixed:
        //         _formats.SelectedItem = 0;
        //         break;
        //     case HexInputFormat.HPrefixed:
        //         _formats.SelectedItem = 1;
        //         break;
        //     case HexInputFormat.NonPrefixed:
        //         _formats.SelectedItem = 2;
        //         break;
        //     case HexInputFormat.Decimal:
        //         _formats.SelectedItem = 3;
        //         break;
        // }

        _ = _format_container.Add(_hex_format);
        _ = _format_container.Add(_the_formats);
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