using libCommunication.Configuration;

namespace MultiTermCLI.Configuration;

public class TerminalSettings {
    public required List<SerialPortSettings> Terminals {get; set;}
}