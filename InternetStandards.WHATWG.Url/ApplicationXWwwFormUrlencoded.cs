using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InternetStandards.WHATWG.Url
{
    public static class ApplicationXWwwFormUrlencoded
    {
        public static IList<(string Name, string Value)> Parse(IEnumerable<byte> input)
        {
            var sequences = new Sequences(input);
            IList<(string Name, string Value)> output = new List<(string name, string value)>();

            while (sequences.MoveNext())
            {
                var bytes = sequences.Current;
                if (bytes!.Count == 0)
                    continue;

                IEnumerable<byte> name;
                IEnumerable<byte> value;
                var indexOfEquals = bytes.IndexOf(0x3D);
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

                name = name.Select(b => b == 0x2B ? (byte)0x20 : b);
                value = value.Select(b => b == 0x2B ? (byte)0x20 : b);

                var nameString = Encoding.UTF8.GetString(PercentEncodedBytes.PercentDecode(name).ToArray());
                var valueString = Encoding.UTF8.GetString(PercentEncodedBytes.PercentDecode(value).ToArray());

                output.Add((nameString, valueString));
            }

            return output;
        }

        public static IList<(string Name, string Value)> Parse(string input) => Parse(Encoding.UTF8.GetBytes(input));

        public static string Serialize(IEnumerable<(string Name, string Value)> tuples,
            Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;

            var output = new StringBuilder();

            foreach (var tuple in tuples)
            {
                var name = PercentEncodedBytes.PercentEncodeAfterEncoding(encoding, tuple.Name, null, true);
                var value = PercentEncodedBytes.PercentEncodeAfterEncoding(encoding, tuple.Value, null, true);

                if (output.Length != 0)
                    output.Append('&');
                output.Append(name);
                output.Append('=');
                output.Append(value);
            }
            return output.ToString();
        }
    }
}
