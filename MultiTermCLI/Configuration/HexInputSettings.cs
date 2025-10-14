namespace MultiTermCLI.Configuration;

public class HexSettings {
    public required HexFormat InputFormat { get; set; }
    public required HexSequenceSeperator Seperator { get; set; }

    /// <summary>
    /// Creates a deep clone of the object.
    /// </summary>
    public HexSettings Clone() {
        return new HexSettings {
            InputFormat = InputFormat,
            Seperator = Seperator,
        };
    }
}