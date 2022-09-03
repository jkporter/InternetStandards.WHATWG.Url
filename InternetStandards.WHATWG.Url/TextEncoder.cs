using System;
using System.Collections.Generic;
using System.Text;

namespace InternetStandards.WHATWG.Url
{
    internal interface ITextEncoder
    {
        byte[] Encode(string input = string.Empty);
        TextEncoderEncodeIntoResult EncodeInto(string source, byte[] destination);
    }
}
