namespace libMultiTerm.Plugin.Models;

public class UserDefinedPayload {
    /// <summary>
    /// A human friendly name of the payload
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// The delegate that generates the payload
    /// </summary>
    public required Func<byte[]> Delegate { get; init; }

    /// <summary>
    /// The payload itself
    /// </summary>
    public byte[] Payload => Delegate.Invoke();
}