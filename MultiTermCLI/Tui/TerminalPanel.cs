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

    private readonly TextView _view;
    public TextView View => _view;

    public Pos X {
        get => _view.X;
        set => _view.X = value;
    }

    public Pos Y {
        get => _view.Y;
        set => _view.Y = value;
    }

    public Dim? Width {
        get => _view.Width;
        set => _view.Width = value;
    }

    public Dim? Height {
        get => _view.Height;
        set => _view.Height = value;
    }

    public TerminalPanel(SerialPortSettings settings) {
        _settings = settings;


        _view = new TextView() {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            ReadOnly = true
        };

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