using Terminal.Gui;
using MultiTermCLI.Configuration;

public class SettingsDialog : Dialog {

    private TabView _tabView;
    private HexInputSettings _inputSettings;

    public SettingsDialog(HexInputSettings inputSettings) : base() {
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

        Tab display_settings = BuildHexSettingsTab();

        _tabView.AddTab(display_settings, true);

        Add(_tabView);

        this.KeyDown += (object? sender, Key e) => {
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

        _container.Add(_format_container);
        _container.Add(_seperator_container);

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
            Text = "Hello World",
            //TabStop = TabBehavior.TabStop
        };

        _container.Add(_example);


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

        RadioGroup _formats = new() {
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

        switch(_inputSettings.InputFormat) {
            case HexInputFormat.ZeroPrefixed:
                _formats.SelectedItem = 0;
                break;
            case HexInputFormat.HPrefixed:
                _formats.SelectedItem = 1;
                break;
            case HexInputFormat.NonPrefixed:
                _formats.SelectedItem = 2;
                break;
            case HexInputFormat.Decimal:
                _formats.SelectedItem = 3;
                break;
        }

        _format_container.Add(_hex_format);
        _format_container.Add(_formats);

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

        RadioGroup _seperators = new() {
            RadioLabels = [
                "Space",
                "Comma"
            ],
            X = 0,
            Y = Pos.Bottom(_hex_seperator) + 1,
            CanFocus = true,
            TabStop = TabBehavior.TabStop
        };

        switch(_inputSettings.Seperator) {
            case HexSeperator.Space:
                _seperators.SelectedItem = 0;
                break;
            case HexSeperator.Comma:
                _seperators.SelectedItem = 1;
                break;
        }

        _seperator_container.Add(_hex_seperator);
        _seperator_container.Add(_seperators);

        return _seperator_container;
    }
}