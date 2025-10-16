[System.AttributeUsage(System.AttributeTargets.Method)]

public class DeviceCommandAttribute : System.Attribute {
    private string Name;

    public DeviceCommandAttribute(string name) {
        Name = name;
    }
}