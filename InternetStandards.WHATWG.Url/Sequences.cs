using System.Collections;
using System.Collections.Generic;

namespace InternetStandards.WHATWG.Url;

internal struct Sequences : IEnumerator<IList<byte>>
{
    private readonly IEnumerator<byte> _input;

    public Sequences(IEnumerable<byte> input)
    {
        _input = input.GetEnumerator();
    }

    private bool _shouldNotMoveNext = false;
    public bool MoveNext()
    {
        if (_shouldNotMoveNext)
            return false;

        Current = new List<byte>();

        while (!(_shouldNotMoveNext = !_input.MoveNext()) && _input.Current != 0x26)
            Current.Add(_input.Current);

        return true;
    }

    public void Reset()
    {
        _input.Reset();
    }

    public IList<byte> Current { get; private set; } = null;

    object IEnumerator.Current => Current;

    public void Dispose()
    {
        _input.Dispose();
    }
}