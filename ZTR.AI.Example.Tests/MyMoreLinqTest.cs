using AutoFixture.NUnit3;
using FluentAssertions;
using ZTR.AI.Example.Linq;

namespace ZTR.AI.Example.Tests;

public class MyMoreLinqTest
{
    [Test,AutoData]
    public void ForEach_InvokesAction_ForEachElement(IReadOnlyCollection<object> testCollection)
    {
        var invoked = new List<object>();
        testCollection.ForEach(d => invoked.Add(d));

        testCollection.Should().BeEquivalentTo(invoked);
    }

    [Test]
    public void ForEach_ThrowsArgumentNullException_WhenSourceIsNull()
    {
        var action = () => ((IEnumerable<object>) null!).ForEach(_ => { });
        action.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void ForEach_ThrowsArgumentNullException_WhenActionIsNull()
    {
        var action = () => Enumerable.Empty<object>().ForEach(null!);
        action.Should().Throw<ArgumentNullException>();
    }
}