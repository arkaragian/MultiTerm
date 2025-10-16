namespace ExamplePlugin;

public class AProtocolPlugin {

    public AProtocolPlugin() {
    }

    [DeviceCommand("Sample Command")]
    public static byte[] SampleCommand() {
        return [0x01, 0x02, 0x03];
    }
}