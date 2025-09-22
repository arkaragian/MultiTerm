using libCommunication.Configuration;
using Terminal.Gui;

namespace MuliTermCLI.Tui;

public class TerminalPanel : TextView {

    private readonly SerialPortSettings _settings;

    public TerminalPanel(SerialPortSettings settings) {
        _settings = settings;
    }
}