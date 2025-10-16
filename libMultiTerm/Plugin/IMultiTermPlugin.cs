namespace libMultiTerm.Plugin;

public interface IMultiTermPlugin {
    /// <summary>
    /// A plugin name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// A Free Form human readable description on that this plugin does.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// A list of commands that generate bytes
    /// </summary>
    //public Dictionary<string, Func<byte[]>> StaticCommands;
}