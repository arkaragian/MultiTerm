using MultiTermCLI.Configuration;
using System.Diagnostics.CodeAnalysis;
using Terminal.Gui;

namespace MultiTermCLI.Tui.SettingsDialog;

public class HexSettingsView : View {

    private readonly Dim _height = 12;

    /// <summary>
    /// Selects between the available hex formats
    /// </summary>
    public RadioGroup HexFormats { get; private set; }

    /// <summary>
    /// Selects between the available hex formats
    /// </summary>
    public RadioGroup Seperators { get; private set; }
    public HexSettings Settings { get; set; }

    public HexSettingsView(HexSettings inputSettings) {
        X = 0;
        Y = 0;
        Width = Dim.Fill();
        Height = _height;
        BorderStyle = LineStyle.Single;
        CanFocus = true;

        Settings = inputSettings;


        Label _lbl = new() {
            Text = "Hex Format:",
            X = 0,
            Y = 0
        };

        HexFormats = new() {
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

        switch (Settings.InputFormat) {
            case HexFormat.ZeroPrefixed:
                HexFormats.SelectedItem = 0;
                break;
            case HexFormat.HPrefixed:
                HexFormats.SelectedItem = 1;
                break;
            case HexFormat.NonPrefixed:
                HexFormats.SelectedItem = 2;
                break;
            case HexFormat.Decimal:
                HexFormats.SelectedItem = 3;
                break;
        }

        _ = Add(_lbl);
        _ = Add(HexFormats);



        //This is a dummy init to suppres the warning. The actual initialisation
        //happens in BuildSeperatorView
        Seperators = new RadioGroup();
        View _seperator_container = BuildSeperatorView();
        _seperator_container.X = Pos.Right(HexFormats);

        _ = Add(_seperator_container);


        TextView _example = new() {
            Title = "Hex Input Example",
            X = 0,
            Y = Pos.Bottom(HexFormats) + 1,
            Width = Dim.Fill(),
            Height = 1,
            ReadOnly = true,
            BorderStyle = LineStyle.Single,
            CanFocus = false,
            Multiline = false,
            Text = Payload.RenderPayload([0xAB, 0xAC], Settings),//Payload.BuildPayload("Hello World", asHex: true, _inputSettings, false, false)
                                                                 //TabStop = TabBehavior.TabStop
        };

        _ = Add(_example);


        // after you construct Formats
        HexFormats.SelectedItemChanged += (object? sender, SelectedItemChangedArgs args) => {
            switch (HexFormats.SelectedItem) {
                case 0: Settings.InputFormat = HexFormat.ZeroPrefixed; break;
                case 1: Settings.InputFormat = HexFormat.HPrefixed; break;
                case 2: Settings.InputFormat = HexFormat.NonPrefixed; break;
                case 3: Settings.InputFormat = HexFormat.Decimal; break;
                default: break;
            }

            _example.Text = Payload.RenderPayload([0xAB, 0xAC], Settings);
        };

        if (Seperators is not null) {

            // after you construct Seperators
            Seperators.SelectedItemChanged += (object? sender, SelectedItemChangedArgs args) => {
                switch (Seperators.SelectedItem) {
                    case 0: Settings.Seperator = HexSequenceSeperator.Space; break;
                    case 1: Settings.Seperator = HexSequenceSeperator.Comma; break;
                    default: break;
                }

                _example.Text = Payload.RenderPayload([0xAB, 0xAC], Settings);
            };

        }
    }


    [MemberNotNull(nameof(Seperators))]
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

        Seperators = new() {
            RadioLabels = [
                "Space",
                "Comma"
            ],
            X = 0,
            Y = Pos.Bottom(_hex_seperator) + 1,
            CanFocus = true,
            TabStop = TabBehavior.TabStop
        };

        switch (Settings.Seperator) {
            case HexSequenceSeperator.Space:
                Seperators.SelectedItem = 0;
                break;
            case HexSequenceSeperator.Comma:
                Seperators.SelectedItem = 1;
                break;
        }

        _ = _seperator_container.Add(_hex_seperator);
        _ = _seperator_container.Add(Seperators);

        return _seperator_container;
    }
}