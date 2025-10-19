using libMultiTerm.Configuration;
using MultiTermCLI.Tui;
using System.Text.Json;
using System.Text.Json.Serialization;
using Terminal.Gui;

namespace MultiTermCLI;

public class Program {
    public static int Main(string[] args) {

        // if (!ConsoleVT.IsVtEnabled()) {
        //     Console.WriteLine("No Virtual Terminal Processing. Enabling virtual terminal");
        //     if (!ConsoleVT.TryEnableVt()) {
        //         Console.WriteLine("No VT Support! Exiting...");
        //         return 0;
        //     }
        // }
        //
        if (args.Length is not 1) {
            Console.WriteLine("MultiTerm Requires One Argument");
        }

        string f = args[0];

        if (!File.Exists(f)) {
            Console.WriteLine("File does not exist");
        }

        string contents = File.ReadAllText(f);

        JsonSerializerOptions options = new() {
            PropertyNameCaseInsensitive = true
        };
        options.Converters.Add(new JsonStringEnumConverter());

        TerminalSettings? settings = JsonSerializer.Deserialize<TerminalSettings>(contents, options);


        if (settings is null) {
            Console.WriteLine("Could not deserialize input configuration");
            return 1;
        }




        Application.Init();                         // set raw mode, alt screen, colors
        Application.Navigation?.AdvanceFocus(NavigationDirection.Forward, TabBehavior.TabStop);
        // optional: make Ctrl+C quit
        Console.TreatControlCAsInput = false;
        Application.QuitKey = Key.C.WithCtrl;   // maps to Command.Quit
                                                // or also wire SIGINT:


        Toplevel top = new();
        Console.CancelKeyPress += (_, e) => {
            e.Cancel = true;
            Application.RequestStop(top);
        };

        // Window w = new Terminal.Gui.Window { X = 0, Y = 0, Width = Dim.Fill(), Height = Dim.Fill() };
        // w.Add(new Label() { X = 2, Y = 1, Title = "Hello From Terminal" });
        // top.Add(w);


        // MultiTermMenu mn = new();
        //
        // _ = top.Add(mn);

        MainTUIWindow win = new MainTUIWindow(settings) {
            //Y = Pos.Bottom(mn)
        };

        _ = top.Add(win);

        Application.Run(top);
        top.Dispose();
        Application.Shutdown();                 // restore terminal
                                                //ConsoleVT.Restore();
                                                // Safety reset for alt screen, attributes, and cursor visibility.



        return 0;
    }
}