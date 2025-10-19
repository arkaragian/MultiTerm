[System.AttributeUsage(System.AttributeTargets.Method)]

public class DeviceCommandAttribute : System.Attribute {
    public string Name { get; private set; }

    public DeviceCommandAttribute(string name) {
        Name = name;
    }
}