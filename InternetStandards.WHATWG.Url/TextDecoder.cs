namespace InternetStandards.WHATWG.Url;

public abstract class TextDecoder :ITextDecoderCommon
{
    public TextDecoder(string label = "utf-8", TextDecoderOptions options = null)
    {
    }

    public abstract string Decode(byte[] input = null, TextDecoderOptions options = null);
    public string Encoding { get; }
    public bool Fatal { get; }
    public bool IgnoreBom { get; }
}