using libMultiTerm.Plugin.Models;
using System.Collections.ObjectModel;
using Terminal.Gui;

namespace MultiTermCLI.Tui.PayloadSelectionDialog;

public class PayloadSelectionDialog : Dialog {

    public List<UserDefinedPayload> Payloads {get; private set;}
    public byte[]? SelectedPayload { get; private set;}

    public PayloadSelectionDialog(List<UserDefinedPayload> payloads) {
        Payloads = payloads;

        Modal = true;
        Title = "Payload Selection";

        IEnumerable<string> a = from item in Payloads select item.Name;
        ObservableCollection<string> samples = new(a.ToList());

        // int index=0;
        // foreach(KeyValuePair<string, Func<byte[]>> kvp in Payloads) {
        //     samples.InsertItem(index++, kvp.Key);
        // }

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
            if (e == Key.Esc) {
                Application.RequestStop(this);
                e.Handled = true;
            }

        };

        lv.KeyDown += (object? sender, Key e) => {
            if (e == Key.Enter) {
                int sel  = lv.SelectedItem;
                byte[] pl = Payloads[sel].Payload;
                SelectedPayload = pl;
                //string[] chosen = lv.Source.ToList().Cast<string>().Where((o, idx) => lv.Source.IsMarked(idx)).ToArray();
                //MessageBox.Query("Chosen", "You chose " + string.Join(",", chosen), "Ok");
                Application.RequestStop(this);
                e.Handled = true;
            }
        };
    }

}