using libCommunication;
using libCommunication.Configuration;
using libCommunication.Foundation;
using Terminal.Gui;

namespace MuliTermCLI.Tui;

public class TerminalPanel : TextView {

    private readonly SerialPortSettings _settings;

    public TerminalPanel(SerialPortSettings settings) {
        _settings = settings;

        Serial port = new(_settings.PortName, _settings.BaudRate, _settings.Parity, _settings.DataBits, _settings.StopBits);
        SerialReadThread srt = new();
        //SerialWriteTread swt = new();
    }
}