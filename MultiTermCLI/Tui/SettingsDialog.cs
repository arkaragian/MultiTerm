using Terminal.Gui;

public class SettingsDialog : Dialog {

    private TabView _tabView;

    public SettingsDialog() : base() {
        //Title = "Settings";
        Modal = true;
        DefaultShadow = ShadowStyle.None;

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
            Height = Dim.Fill()
        };

        Tab display_settings = new() {
            Title = "Display",
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };

        Button test = new Button() {
            Title = "_Test"
        };

        display_settings.Add(test);

        Tab display_settings2 = new() {
            Title = "Capture",
            X = 10,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };

        Button test2 = new Button() {
            Title = "_Test"
        };

        display_settings2.Add(test2);


        _tabView.AddTab(display_settings, true);
        _tabView.AddTab(display_settings2, false);

        Add(_tabView);

        // Button close = new Button() {
        //     Title = "_Close"
        // };
        // close.Accepting += (object? sender, CommandEventArgs args) => Application.RequestStop(this);
        // AddButton(close);

        this.KeyDown += (object? sender, Key e) => {
            if (e == Key.Esc || e == Key.Q) {
                Application.RequestStop(this);
                e.Handled = true;
            }
        };

    }
}