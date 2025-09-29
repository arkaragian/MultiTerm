using libCommunication;
using libCommunication.Configuration;
using libCommunication.Foundation;
using Terminal.Gui;

namespace MultiTermCLI.Tui;

public class TerminalPanel : TextView {

    private readonly SerialPortSettings _settings;

    public TerminalPanel(SerialPortSettings settings) {
        _settings = settings;

        Serial port = new(_settings.PortName, _settings.BaudRate, _settings.Parity, _settings.DataBits, _settings.StopBits);

        ProtocolDescription desc = new() {
            StandardHeaderLength = -1,
            EOLByte = null,
            EOLByteSequence = null,
            SupportsMultiplexing = false,
            BisectingLogic = static a => { return [a]; }
        };


        SerialReadThread srt = new(_settings.PortName, port, desc);
        //SerialWriteTread swt = new();
    }
}