using libCommunication.Configuration;

namespace MultiTermCLI.Configuration;

public class TerminalConfiguration {
    public required NetTransmissionProtocol ConnectionType { get; set; }
    public SerialPortSettings? SerialPortSettings { get; set; }
    public NetworkConnectionSettings? NetworkConnectionSettings { get; set; }

    public bool IsValidSetting() {
        if(ConnectionType is NetTransmissionProtocol.TCP) {
        }

        return true;
    }
}

public class TerminalSettings {
    public required List<TerminalConfiguration> Terminals { get; set; }
}