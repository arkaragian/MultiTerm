using libMultiTerm.Configuration;
using libCommunication;
using libCommunication.Configuration;
using libCommunication.Foundation;
using libCommunication.interfaces;
using Terminal.Gui;
using libCommunication.Serial;
using libMultiTerm;
using libMultiTerm.Plugin;

namespace MultiTermCLI.Tui;

//public sealed class TerminalPanel : IDisposable {
public sealed class TerminalPanel : View {

    private readonly TerminalConfiguration _settings;
    private readonly CancellationTokenSource _cts;
    private readonly IReadThread _read_thread;
    private readonly IWriteThread _write_thread;
    private readonly Thread _terminalLoop;
    private readonly PluginLoader _plugin_loader;

    private bool _disposed;

    // public View Frame { get; }
    public TextView View { get; }

    private readonly TerminalInputLine _input;

    //private Dictionary<string, Func<byte[]>>? _pluginFunctions;

    public TerminalPanel(TerminalConfiguration settings) {
        _settings = settings;
        if (_settings.HexInputSettings is null) {
            _settings.HexInputSettings = new HexSettings() {
                InputFormat = HexFormat.ZeroPrefixed,
                Seperator = HexSequenceSeperator.Space
            };
        }

        if (_settings.HexDisplaySettings is null) {
            _settings.HexDisplaySettings = new HexSettings() {
                InputFormat = HexFormat.ZeroPrefixed,
                Seperator = HexSequenceSeperator.Space
            };
        }

        if (!_settings.IsValidSetting()) {
            throw new InvalidOperationException("Settings are not valid");
        }

        int _input_height = 3;

        X = 0;
        Y = 0;
        Width = Dim.Fill();
        Height = Dim.Fill();
        CanFocus = true;


        View = new TextView() {
            Title = _settings.Title,
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(margin: _input_height),
            ReadOnly = true,
            BorderStyle = LineStyle.Single,
            CanFocus = false,
        };

        _input = new TerminalInputLine(_settings.HexInputSettings) {
            Title = "Input",
            X = 0,
            Y = Pos.AnchorEnd(_input_height),
            Width = Dim.Fill(),
            Height = _input_height,
            BorderStyle = LineStyle.Single,
            ColorScheme = new ColorScheme() {
                Normal = new Terminal.Gui.Attribute(Color.Green, Color.Black),      // Text = white, background = black
                Focus = new Terminal.Gui.Attribute(Color.White, Color.Black),       // Same text colors when focused
            },
            CanFocus = true,
            TabStop = TabBehavior.TabStop
        };

        _ = Add(View);
        _ = Add(_input);

        Serial? port = null;
        CommTCPClient? client = null;
        if (_settings.ConnectionType is ConnectionType.Network) {
            client = new CommTCPClient(_settings.NetworkConnectionSettings!.RemoteAddress, _settings.NetworkConnectionSettings.RemotePort);
        } else {
            port = new(_settings.SerialPortSettings!.PortName, _settings.SerialPortSettings.BaudRate, _settings.SerialPortSettings.Parity, _settings.SerialPortSettings.DataBits, _settings.SerialPortSettings.StopBits) {
                ReadTimeout = 200
            };
        }

        ProtocolDescription desc = new() {
            StandardHeaderLength = -1,
            EOLByte = null,
            EOLByteSequence = null,
            SupportsMultiplexing = false,
            BisectingLogic = static a => { return [a]; }
        };


        if (_settings.ConnectionType is ConnectionType.Network) {
            _read_thread = new TCPReadThread("TCPThread", client!, desc);
        } else {
            _read_thread = new SerialReadThread(_settings.Title, port!, desc);
        }

        Task<Exception?> r = _read_thread.Start();
        if (r.Result is not null) {
            throw r.Result;
        }


        if (_settings.ConnectionType is ConnectionType.Network) {
            _write_thread = new TCPWriteThread("TCPThread", client!, desc);
        } else {
            _write_thread = new SerialWriteThread(_settings.Title, port!, desc);
        }
        r = _write_thread.Start();
        if (r.Result is not null) {
            throw r.Result;
        }


        _plugin_loader = new PluginLoader();

        _input.KeyDown += (object? sender, Key e) => {
            if (e == Key.Enter) {
                byte[]? text = _input.BuildPayload();

                if (text is null) {
                    _ = MessageBox.ErrorQuery("Error", "Invalid Input", buttons: ["OK"]);
                    e.Handled = true;
                    return;
                }

                // handle the completed input here
                _input.Text = "";

                libCommunication.Command cmd = new(text, LayerCommand.None, null);
                _write_thread.Addtoqueue(cmd, handle: null, CancellationToken.None);

                // optional: suppress default behavior
                e.Handled = true;
            }
        };

        this.KeyDown += (object? sender, Key e) => {
            if (e == Key.F1) {
                if (_settings.HexInputSettings is null) {
                    throw new InvalidOperationException("Null hex input settings!");
                }
                SettingsDialog.SettingsDialog dialog = new(_settings.HexInputSettings.Clone(), _settings.HexDisplaySettings.Clone()) {
                    Title = _settings.Title + " Settings",
                };
                Application.Run(dialog);

                if (dialog.Accepted) {
                    _settings.HexInputSettings = dialog.ResultInputSettings;
                    _settings.HexDisplaySettings = dialog.ResultDisplaySettings;
                    _input.InputSettings = _settings.HexInputSettings;
                    _ = MessageBox.Query("Confirm", "New Settings Applied", buttons: ["OK"]);
                } else {
                    _ = MessageBox.Query("Confirm", "No Settings Applied", buttons: ["OK"]);
                }
                e.Handled = true;
                return;
            }

            if (e == Key.F2) {
                OpenDialog opendlg = new();
                Application.Run(opendlg);
                if (opendlg.FilePaths.Count() > 0) {
                    Exception? ex = _plugin_loader.LoadPlugin(opendlg.FilePaths[0]);

                    if (ex is not null) {
                        _ = MessageBox.ErrorQuery("Error", $"No Plugin Loaded: {ex.Message}", buttons: ["OK"]);
                        e.Handled = true;
                        return;
                    }

                }

                e.Handled = true;
                return;
            }

            if (e == Key.F3) {
                if (_plugin_loader.Payloads is null) {
                    _ = MessageBox.ErrorQuery("Error", "No Plugin Loaded", buttons: ["OK"]);
                    e.Handled = true;
                    return;
                }
                PayloadSelectionDialog.PayloadSelectionDialog psld = new(_plugin_loader.Payloads) {
                    ColorScheme = ColorScheme
                };
                Application.Run(psld);
                if (psld.SelectedPayload is not null) {
                    string str = Payload.RenderPayload(psld.SelectedPayload, _settings.HexInputSettings);

                    Application.Invoke(() => {
                        _input.Input.Text = str;
                    });
                }
                e.Handled = true;
                return;
            }
        };

        _cts = new();

        _terminalLoop = new Thread(() => TerminalLoopLogic(_cts.Token));
        _terminalLoop.Start();

        // Ensure the input is ready to type into.
        Application.Invoke(() => _input.Input.SetFocus());
    }

    private void TerminalLoopLogic(CancellationToken ct) {
        IPacketSourceSink<byte[]>? sink = _read_thread.Sink;

        if (sink is null) {
            throw new InvalidOperationException("Sink is null!");
        }
        while (!ct.IsCancellationRequested) {
            try {
                (byte[] payload, _) = sink.Take(ct);
                string text;
                if (_input.DisplayHex) {
                    Console.WriteLine("Rendering Hex");
                    text = Payload.RenderPayload(payload, _settings.HexDisplaySettings!);
                } else {
                    Console.WriteLine("Rendering String");
                    text = Payload.RenderStringPayload(payload);
                }

                Application.Invoke(() => {
                    View.Text += text;
                    View.Text += "\n";
                    View.MoveEnd(); // optional: scrolls to bottom
                });
            } catch (OperationCanceledException) {
                break;
            }

        }
        _read_thread.Stop();
    }

    public new void Dispose() {
        if (_disposed) {
            return;
        }

        _disposed = true;
        _cts.Cancel();
        _write_thread.Stop();
        _terminalLoop.Join();
        _cts.Dispose();
        View.Dispose();
        base.Dispose();
    }

}