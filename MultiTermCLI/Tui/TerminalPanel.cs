using libCommunication;
using libCommunication.Configuration;
using libCommunication.Foundation;
using libCommunication.interfaces;
using Terminal.Gui;

namespace MultiTermCLI.Tui;

public sealed class TerminalPanel : IDisposable {

    private readonly SerialPortSettings _settings;
    private readonly CancellationTokenSource _cts;
    private readonly SerialReadThread _srt;
    private readonly Thread _terminalLoop;

    private bool _disposed;

    private View _frame;
    public View Frame => _frame;

    private TextView _view;
    public TextView View => _view;

    private TextField _input;
    public TextField Input => _input;

    private readonly Lock _writeLock;

    public Pos X {
        get => _frame.X;
        set => _frame.X = value;
    }

    public Pos Y {
        get => _frame.Y;
        set => _frame.Y = value;
    }

    public Dim? Width {
        get => _frame.Width;
        set => _frame.Width = value;
    }

    public Dim? Height {
        get => _frame.Height;
        set => _frame.Height = value;
    }

    public TerminalPanel(SerialPortSettings settings) {
        _settings = settings;
        _writeLock = new();

        int _input_height = 3;


        _frame = new View() {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            CanFocus = true
        };

        _view = new TextView() {
            Title = settings.PortName,
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(margin: _input_height),
            ReadOnly = true,
            BorderStyle = LineStyle.Single,
            CanFocus = false,
        };

        _input = new TextField() {
            Title = "Input",
            X = 0,
            Y = Pos.AnchorEnd(_input_height),
            Width = Dim.Fill(),
            Height = _input_height,
            BorderStyle = LineStyle.Single,
            ReadOnly = false,
            CanFocus = true
        };


        _frame.Add(_view);
        _frame.Add(_input);


        Serial port = new(_settings.PortName, _settings.BaudRate, _settings.Parity, _settings.DataBits, _settings.StopBits) {
            ReadTimeout = 200
        };

        Console.WriteLine("Hello");

        ProtocolDescription desc = new() {
            StandardHeaderLength = -1,
            EOLByte = null,
            EOLByteSequence = null,
            SupportsMultiplexing = false,
            BisectingLogic = static a => { return [a]; }
        };


        _srt = new(_settings.PortName, port, desc);
        Task<Exception?> r = _srt.Start();
        if (r.Result is not null) {
            throw r.Result;
        }

        _cts = new();

        _terminalLoop = new Thread(() => TerminalLoopLogic(_cts.Token));
        _terminalLoop.Start();

        // Ensure the input is ready to type into.
        Application.Invoke(() => _input.SetFocus());
    }

    private void TerminalLoopLogic(CancellationToken ct) {
        IPacketSourceSink<byte[]>? sink = _srt.Sink;

        if (sink is null) {
            throw new InvalidOperationException("Sink is null!");
        }
        while (!ct.IsCancellationRequested) {
            try {
                (byte[] payload, _) = sink.Take(ct);
                string text = System.Text.Encoding.ASCII.GetString(payload);

                Application.Invoke(() => {
                    _view.Text += text;
                    _view.MoveEnd(); // optional: scrolls to bottom
                });
            } catch (OperationCanceledException) {
                break;
            }

        }
    }


    public void Dispose() {
        if (_disposed) {
            return;
        }

        _disposed = true;
        _cts.Cancel();
        _terminalLoop.Join();
        _cts.Dispose();
        _view.Dispose();
    }

}