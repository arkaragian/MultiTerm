using MultiTermCLI.Configuration;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace MultiTermCLI;

public static class Payload {
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