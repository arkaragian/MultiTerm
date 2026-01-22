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
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip
        };
        options.Converters.Add(new JsonStringEnumConverter());

        TerminalSettings? settings = JsonSerializer.Deserialize<TerminalSettings>(contents, options);


        if (settings is null) {
            Console.WriteLine("Could not deserialize input configuration");
            return 1;
        }




        Application.Init();                         // set raw mode, alt screen, colors
        // optional: make Ctrl+C quit
        // Console.TreatControlCAsInput = false;
         Application.QuitKey = Key.Q.WithCtrl;   // maps to Command.Quit
        //                                         // or also wire SIGINT:


        Toplevel top = new();
        Console.CancelKeyPress += (_, e) => {
            e.Cancel = true;
            Application.RequestStop(top);
        };


        MainTUIWindow win = new(settings);

        _ = top.Add(win);

        Application.Run(top);
        top.Dispose();
        Application.Shutdown();                 // restore terminal
                                                //ConsoleVT.Restore();
                                                // Safety reset for alt screen, attributes, and cursor visibility.



        return 0;
    }
}