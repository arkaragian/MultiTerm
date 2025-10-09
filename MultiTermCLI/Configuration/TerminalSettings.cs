using libCommunication.Configuration;

namespace MultiTermCLI.Configuration;

public class TerminalConfiguration {
    public required ConnectionType ConnectionType { get; set; }
    public SerialPortSettings? SerialPortSettings { get; set; }
    public NetworkConnectionSettings? NetworkConnectionSettings { get; set; }

    public bool IsValidSetting() {
        if (ConnectionType is ConnectionType.Network) {
            if(NetworkConnectionSettings is not null) {
                return true;
            }
        }

        if (ConnectionType is ConnectionType.Serial) {
            if(SerialPortSettings is not null) {
                return true;
            }
        }

        return false;
    }

    public string Title {
        get {
            if (ConnectionType is ConnectionType.Network) {
                return NetworkConnectionSettings.RemoteAddress;
            }
            return SerialPortSettings.PortName;
        }
    }
}

public class TerminalSettings {
    public required List<TerminalConfiguration> Terminals { get; set; }
}