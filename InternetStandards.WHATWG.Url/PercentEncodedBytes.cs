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
            var inputArray = input.ToArray();

            var index = 0;

            bool NotInRange()
            {
                var nextTwoBytesAfter = inputArray.Skip(index + 1).Take(2).ToArray();
                return nextTwoBytesAfter.Length == 2 && nextTwoBytesAfter.All(b =>
                    !(b >= 0x30 && b <= 0x39 || b >= 0x41 && b <= 0x46 || b >= 0x61 && b <= 0x66));
            }

            for (; index < inputArray.Length; index++)
            {
                if (inputArray[index] != 0x25 || NotInRange())
                {
                    yield return inputArray[index];
                }
                else
                {
                    var bytePoint = Encoding.UTF8.GetString(new[] {inputArray[++index], inputArray[++index]});
                    yield return Convert.ToByte(bytePoint, 16);
                }
            }
        }

        public static IEnumerable<byte> PercentDecode(string input) => PercentDecode(Encoding.UTF8.GetBytes(input));
    }
}
