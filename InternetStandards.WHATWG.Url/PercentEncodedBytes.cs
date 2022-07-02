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
                buffer.Clear();
                yield return Convert.ToByte(Encoding.UTF8.GetString(bytes), 16);
            }
        }

        public static IEnumerable<byte> PercentDecode(string input) => PercentDecode(Encoding.UTF8.GetBytes(input));
    }
}