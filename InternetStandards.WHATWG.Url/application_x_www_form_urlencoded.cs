using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace InternetStandards.WHATWG.Url
{
    public static class application_x_www_form_urlencoded
    {
        public static IList<(string Name, string Value)> Parse(IEnumerable<byte> input)
        {
            var sequences = Split(input);
            IList<(string Name, string Value)> output = new List<(string name, string value)>();

            foreach (var bytes in sequences.Select(sBytes => sBytes.ToArray()))
            {
                if (bytes.Length == 0)
                    continue;

                IEnumerable<byte> name;
                IEnumerable<byte> value;
                var indexOfEquals = Array.IndexOf(bytes, (byte)0x3D);
                if (indexOfEquals != -1)
                {
                    name = bytes.Take(indexOfEquals);
                    value = bytes.Skip(indexOfEquals + 1);
                }
                else
                {
                    name = bytes;
                    value = Enumerable.Empty<byte>();
                }

                name = name.Select(b => b == 0x2B ? (byte) 0x20 : b);
                value = value.Select(b => b == 0x2B ? (byte) 0x20 : b);

                var nameString = Encoding.UTF8.GetString(PercentEncodedBytes.PercentDecode(name).ToArray());
                var valueString = Encoding.UTF8.GetString(PercentEncodedBytes.PercentDecode(value).ToArray());

                output.Add((nameString, valueString));
            }

            return output;
        }

        public static IList<(string Name, string Value)> Parse(string input)
        {
            return Parse(Encoding.UTF8.GetBytes(input));
        }

        private static IEnumerable<IEnumerable<byte>> Split(IEnumerable<byte> input)
        {
            using var enumerator = input.GetEnumerator();
        
            if (!enumerator.MoveNext())
            {
                yield return Enumerable.Empty<byte>();
                yield break;
            }

            do
            {
                yield return TakeUntilByte0x26(enumerator);
            } while (enumerator.MoveNext());
        }

        private static IEnumerable<byte> TakeUntilByte0x26(IEnumerator<byte> input)
        {
            do
            {
                if (input.Current == 0x26)
                    yield break;
                yield return input.Current;
            } while (input.MoveNext());
        }

        public static string ByteSerializer(IEnumerable<byte> input)
        {
            var output = new StringBuilder();
            foreach (var @byte in input)
            {
                if (@byte == 0x20)
                {
                    output.Append('+');
                }
                else if (@byte == 0x2A || @byte == 0x2D || @byte == 0x2E || @byte >= 0x30 && @byte <= 0x39 ||
                         @byte >= 0x41 && @byte <= 0x5A || @byte == 0x5F || @byte >= 0x61 && @byte <= 0x7A)
                {
                    output.Append((char)@byte);
                }
                else
                {
                    output.Append('%');
                    output.Append(@byte.ToString("X2"));
                }
            }

            return output.ToString();
        }

        public static string Serializer(ICollection<(string Name, object Value)> tuples,
            Encoding encodingOverride = null)
        {
            var encoding = Encoding.UTF8;
            if (encodingOverride != null)
            {
                throw new NotImplementedException();
            }

            var output = new StringBuilder();

            var firstPair = true;
            foreach (var tuple in tuples)
            {
                var name = ByteSerializer(encoding.GetBytes(tuple.Name));
                var value = tuple.Value;

                if (value is FileInfo file) value = file.Name;

                value = ByteSerializer(encoding.GetBytes((string)value));

                if (!firstPair)
                    output.Append('&');
                output.Append(name);
                output.Append('=');
                output.Append(value);

                if (firstPair)
                    firstPair = false;
            }
            return output.ToString();
        }
    }
}
