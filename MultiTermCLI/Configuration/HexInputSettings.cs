namespace MultiTermCLI.Configuration;

public enum HexInputFormat {
    ZeroPrefixed,
    HPrefixed,
    NonPrefixed,
    Decimal
}

public enum HexSeperator {
    Space,
    Comma
}

public class HexInputSettings {
    public required HexInputFormat InputFormat { get; set; }
    public required HexSeperator Seperator { get; set; }
}