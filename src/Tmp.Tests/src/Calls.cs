using System.Collections;

namespace Tmp.Tests;

public class Calls : IEnumerable<int>
{
    private readonly List<int> _calls = [];

    public void Log(int number)
    {
        _calls.Add(number);
    }

    public void AssertOrderIs(IEnumerable<int> order)
    {
        Assert.That(_calls, Is.EqualTo(order));
    }

    public IEnumerator<int> GetEnumerator()
    {
        return _calls.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}