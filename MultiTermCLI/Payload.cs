using MultiTermCLI.Configuration;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace MultiTermCLI;

/// <summary>
/// Utility Functions to deal with payload conversion
/// </summary>
public static class Payload {

    /// <summary>
    /// Converts a payload received externally to text that can be displayed by the
    /// application.
    /// </summary>
    public static string RenderPayload(byte[]? payload, HexSettings settings) {
        if (payload is null) {
            return string.Empty;
        }

        if (payload.Length is 0) {
            return string.Empty;
        }

        char sep = settings.Seperator switch {
            HexSequenceSeperator.Space => ' ',
            HexSequenceSeperator.Comma => ',',
            _ => ' '
        };

        StringBuilder sb = new();

        foreach (byte b in payload) {
            switch (settings.InputFormat) {
                case HexFormat.ZeroPrefixed:
                    _ = sb.Append("0x");
                    _ = sb.Append(b.ToString("X2", CultureInfo.InvariantCulture));
                    break;
                case HexFormat.HPrefixed:
                    _ = sb.Append('h');
                    _ = sb.Append(b.ToString("X2", CultureInfo.InvariantCulture));
                    break;
                case HexFormat.NonPrefixed:
                    _ = sb.Append(b.ToString("X2", CultureInfo.InvariantCulture));
                    break;
                case HexFormat.Decimal:
                    _ = sb.Append(b.ToString(CultureInfo.InvariantCulture));
                    break;
            }

            sb.Append(sep);

        }

        _ = sb.Remove(sb.Length - 1, 1);

        return sb.ToString();
    }

    public static byte[]? BuildPayload(string? input, bool asHex, HexSettings settings, bool sendCR, bool sendLF) {

        if (input is null) {
            return null;
        }

        if (asHex) {
            List<byte> result = [];

            char sep = settings.Seperator switch {
                HexSequenceSeperator.Space => ' ',
                HexSequenceSeperator.Comma => ',',
                _ => throw new NotImplementedException(),
            };

            string regex = settings.InputFormat switch {
                HexFormat.ZeroPrefixed => @"^0x[0-9A-Fa-f]{2}$",
                HexFormat.HPrefixed => @"^h[0-9A-Fa-f]{2}$",
                HexFormat.NonPrefixed => @"^[0-9A-Fa-f]{2}$",
                HexFormat.Decimal => @"^(?:25[0-5]|2[0-4][0-9]|1?[0-9]{1,2})$",
                _ => throw new NotImplementedException(),
            };

            Regex rgo = new(regex);

            string[] string_bytes = input.Split(sep);

            foreach (string sb in string_bytes) {
                bool ok = rgo.IsMatch(sb);
                if (!ok) {
                    return null;
                }

                byte value = settings.InputFormat switch {
                    HexFormat.ZeroPrefixed => Convert.ToByte(sb[2..], 16),
                    HexFormat.HPrefixed => Convert.ToByte(sb[1..], 16),
                    HexFormat.NonPrefixed => Convert.ToByte(sb, 16),
                    HexFormat.Decimal => byte.Parse(sb, CultureInfo.InvariantCulture),
                    _ => throw new NotImplementedException(),
                };
                result.Add(value);
            }


            if (sendCR) {
                result.Add((byte)'\r');
            }
            if (sendLF) {
                result.Add((byte)'\n');
            }

            return [.. result];

        } else {

            if (sendCR) {
                input += (byte)'\r';
            }
            if (sendLF) {
                input += (byte)'\n';
            }

            return Encoding.ASCII.GetBytes(input);
        }

    }
}