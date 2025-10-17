using System.Collections.ObjectModel;
using Terminal.Gui;

namespace MultiTermCLI.Tui.PayloadSelectionDialog;

public class PayloadSelectionDialog : Dialog {

    public PayloadSelectionDialog() {

        Modal = true;
        Title = "Payload Selection";

        ObservableCollection<string> samples = [
            "aris",
            "karagian",
        ];

        ListView lv = new() {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(margin: 3),
            BorderStyle = LineStyle.Single,
        };

        lv.SetSource<string>(samples);

        FrameView fv = new() {
            Title = "Search",
            BorderStyle = LineStyle.Single,
            X = 0,
            Y = Pos.Bottom(lv),
            Width = Dim.Fill(),
            Height = 3
        };

        TextField tf = new() {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
        };


        fv.Add(tf);

        Add(lv);
        Add(fv);

        KeyDown += (sender, e) => {
            if (e == Key.Esc || e == Key.Q) {
                Application.RequestStop(this);
                e.Handled = true;
            }
        };
    }

}