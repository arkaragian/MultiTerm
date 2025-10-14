using libCommunication;
using libCommunication.Configuration;
using libCommunication.Foundation;
using libCommunication.interfaces;
using MultiTermCLI;
using MultiTermCLI.Configuration;
using MultiTermCLI.Tui.SettingsDialog;
using System.Text;
using Terminal.Gui;

namespace MultiTermCLI.Tui;

public sealed class TerminalPanel : IDisposable {

    private readonly TerminalConfiguration _settings;
    private readonly CancellationTokenSource _cts;
    private readonly IReadThread _read_thread;
    private readonly IWriteThread _write_thread;
    private readonly Thread _terminalLoop;

    private bool _disposed;

    public View Frame { get; }
    public TextView View { get; }

    private readonly TerminalInputLine _input;

    private readonly Lock _writeLock;

    public Pos X {
        get => Frame.X;
        set => Frame.X = value;
    }

    public Pos Y {
        get => Frame.Y;
        set => Frame.Y = value;
    }

    public Dim? Width {
        get => Frame.Width;
        set => Frame.Width = value;
    }

    public Dim? Height {
        get => Frame.Height;
        set => Frame.Height = value;
    }

    public TabBehavior? TabStop {
        get => Frame.TabStop;
        set => Frame.TabStop = value;
    }

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
        _writeLock = new();

        if (!_settings.IsValidSetting()) {
            throw new InvalidOperationException("Settings are not valid");
        }

        int _input_height = 3;


        Frame = new View() {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            CanFocus = true,
            //TabStop = TabBehavior.TabStop
        };

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
            TabStop = TabBehavior.TabStop
        };

        _ = Frame.Add(View);
        _ = Frame.Add(_input.View);

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

        Frame.KeyDown += (object? sender, Key e) => {
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
                    text = Payload.RenderPayload(payload, _settings.HexDisplaySettings);
                } else {
                    text = Encoding.ASCII.GetString(payload);
                }

                Application.Invoke(() => {
                    View.Text += text;
                    View.MoveEnd(); // optional: scrolls to bottom
                });
            } catch (OperationCanceledException) {
                break;
            }

        }
        _read_thread.Stop();
    }

    public void FocusInput() {
        Application.Invoke(() => _input.Input.SetFocus());
    }

    public void Dispose() {
        if (_disposed) {
            return;
        }

        _disposed = true;
        _cts.Cancel();
        _write_thread.Stop();
        _terminalLoop.Join();
        _cts.Dispose();
        View.Dispose();
    }

}