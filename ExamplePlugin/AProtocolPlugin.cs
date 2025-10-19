namespace ExamplePlugin;

public class AProtocolPlugin {

    public AProtocolPlugin() {
    }

    [DeviceCommand("Sample Command")]
    public static byte[] SampleCommand() {
        return [0x01, 0x02, 0x03];
    }

    [DeviceCommand("Command Command Sample")]
    public static byte[] SampleCommand2() {
        return [0x01, 0x02, 0x03];
    }
}