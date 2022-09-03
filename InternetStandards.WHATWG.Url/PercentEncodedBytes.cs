using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InternetStandards.WHATWG.Url
{
    public static class PercentEncodedBytes
    {
        public static string PercentEncode(byte @byte) => $"%{@byte:X2}";

        public static IEnumerable<byte> PercentDecode(IEnumerable<byte> input)
        {
            var buffer = new Queue<byte>(2);
            var bytes = new byte[2];

            using var enumerator = input.GetEnumerator();
            while (buffer.Count > 0 || enumerator.MoveNext())
            {
                var current = buffer.Count > 0 ? buffer.Dequeue() : enumerator.Current;
                if (current != 0x25)
                {
                    yield return current;
                    continue;
                }

                while (buffer.Count < 2 && enumerator.MoveNext())
                    buffer.Enqueue(enumerator.Current);

                if (buffer.Count != 2 || !buffer.All(@byte =>
                        @byte is >= 0x30 and <= 0x39 or >= 0x41 and <= 0x46 or >= 0x61 and <= 0x66))
                {
                    yield return current;
                    continue;
                }

                buffer.CopyTo(bytes, 0);
                yield return Convert.ToByte(Encoding.UTF8.GetString(bytes), 16);
                buffer.Clear();
            }
        }

        public static IEnumerable<byte> PercentDecode(string input) => PercentDecode(Encoding.UTF8.GetBytes(input));

        public static string PercentEncodeAfterEncoding(Encoding encoding, string input, object percentEncodeSet,
            bool spaceAsPlus = false)
        {
            var inputQueue = new Queue<byte>(encoding.GetBytes(input));
            var output = new StringBuilder();

            while (inputQueue.Count > 0)
            {
                var @byte = inputQueue.Dequeue();
                switch (@byte)
                {
                    case 0x20 when spaceAsPlus:
                        output.Append('+');
                        break;
                    case 0x2A or 0x2D or 0x2E or >= 0x30 and <= 0x39 or >= 0x41 and <= 0x5A or 0x5F
                        or >= 0x61 and <= 0x7A:
                        output.Append((char) @byte);
                        break;
                    default:
                        output.Append(PercentEncode(@byte));
                        break;
                }
            }

            return output.ToString();
        }
    }
}