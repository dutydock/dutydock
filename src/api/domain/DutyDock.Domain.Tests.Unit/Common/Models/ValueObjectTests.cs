using DutyDock.Domain.Common.Models.ValueObjects;
using Xunit;

namespace DutyDock.Domain.Tests.Unit.Common.Models;

public class ValueObjectTests
{
    private class AValueObject : ValueObject
    {
        private int PropertyA { get; }

        private string PropertyB { get; }

        public AValueObject(int propertyA, string propertyB)
        {
            PropertyA = propertyA;
            PropertyB = propertyB;
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return PropertyA;
            yield return PropertyB;
        }
    }

    private class BValueObject : ValueObject
    {
        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield break;
        }
    }
    
    [Fact]
    public void Equals_WhenSameProperties_ShouldBe()
    {
        var object1 = new AValueObject(3, "hallo");
        var object2 = new AValueObject(3, "hallo");

        Assert.True(object1.Equals(object2));
        Assert.True(object1 == object2);
        Assert.False(object1 != object2);
    }
    
    [Fact]
    public void Equals_WhenDifferentProperties_ShouldNotBe()
    {
        var object1 = new AValueObject(4, "hallo");
        var object2 = new AValueObject(5, "world");

        Assert.False(object1.Equals(object2));
        Assert.False(object1 == object2);
        Assert.True(object1 != object2);
    }
    
    [Fact]
    public void Equals_WhenDifferentTypes_ShouldNotBe()
    {
        var object1 = new AValueObject(4, "hallo");
        var object2 = new BValueObject();

        Assert.False(object1.Equals(object2));
        Assert.False(object1 == object2);
        Assert.True(object1 != object2);
    }
}