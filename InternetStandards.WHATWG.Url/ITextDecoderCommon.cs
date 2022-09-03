using System.Security.Cryptography.X509Certificates;

namespace InternetStandards.WHATWG.Url;

public interface ITextDecoderCommon
{
    string Encoding { get; }
    bool Fatal { get; }
    bool IgnoreBom { get; }
}

public class TextDecoderOptions
{
    public bool Fatal { get; set; }
    public bool IgnoreBom { get; set; }
}