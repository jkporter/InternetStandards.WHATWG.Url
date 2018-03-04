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
            var twoByteBuffer = new byte[2];

            bool NotInRange()
            {
                return twoByteBuffer.All(b =>
                    !(b >= 0x30 && b <= 0x39 || b >= 0x41 && b <= 0x46 || b >= 0x61 && b <= 0x66));
            }

            using (var enumerator = input.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current != 0x25)
                        yield return enumerator.Current;

                    int twoByteBufferLength;
                    for (twoByteBufferLength = 0;
                        twoByteBufferLength < 2 && enumerator.MoveNext();
                        twoByteBufferLength++)
                        twoByteBuffer[twoByteBufferLength] = enumerator.Current;

                    if (twoByteBufferLength != 2 || NotInRange())
                    {
                        yield return 0x25;
                        for (var index = 0; index < twoByteBufferLength; index++)
                            yield return twoByteBuffer[index];
                    }
                    else
                    {
                        yield return Convert.ToByte(Encoding.UTF8.GetString(twoByteBuffer), 16);
                    }
                }
            }
        }

        public static IEnumerable<byte> PercentDecode(string input) => PercentDecode(Encoding.UTF8.GetBytes(input));
    }
}
