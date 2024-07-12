using System.Text;
using DutyDock.Domain.Common.Models.Entities;
using Xunit;

namespace DutyDock.Domain.Tests.Unit.Common.Models;

public class EntityTests
{
    private class CustomEntity : Entity
    {
        public CustomEntity()
        {
        }

        public CustomEntity(string id) : base(id)
        {
        }
    }

    private class DerivedEntity : Entity
    {
    }
    
    [Fact]
    public void Equals_WhenDifferentType_ShouldNotBe()
    {
        var entity = new CustomEntity();
        var builder = new StringBuilder();

        // ReSharper disable once SuspiciousTypeConversion.Global
        Assert.False(entity.Equals(builder));
    }
    
    [Fact]
    public void Equals_WhenNull_ShouldNotBe()
    {
        var entity = new CustomEntity();
        Assert.False(entity.Equals(null));
    }
    
    [Fact]
    public void Equals_WhenDerivedType_ShouldNotBe()
    {
        var entity = new CustomEntity();
        var derivedEntity = new DerivedEntity();

        Assert.False(entity.Equals(derivedEntity));
    }
    
    [Fact]
    public void Equals_WhenMissingIdentity_ShouldNotBe()
    {
        var entity1 = new CustomEntity();
        var entity2 = new CustomEntity();

        Assert.False(entity1.Equals(entity2));
    }
    
    [Fact]
    public void Equals_WithDifferentIdentity_ShouldNotBe()
    {
        var entity1 = new CustomEntity();
        var entity2 = new CustomEntity();

        Assert.False(entity1.Equals(entity2));
    }
    
    [Fact]
    public void Equals_WithSameReference_ShouldBe()
    {
        var entity = new CustomEntity();

        // ReSharper disable once EqualExpressionComparison
        Assert.True(entity.Equals(entity));
    }
    
    [Fact]
    public void Equals_WithSameIdentity_ShouldBe()
    {
        var id = Guid.NewGuid().ToString();

        var entity1 = new CustomEntity(id);
        var entity2 = new CustomEntity(id);

        Assert.True(entity1.Equals(entity2));
    }
    
    [Fact]
    public void GetHashCode_WhenEqual_ShouldBeSame()
    {
        var id = Guid.NewGuid().ToString();

        var entity1 = new CustomEntity(id);
        var entity2 = new CustomEntity(id);

        Assert.Equal(entity1.GetHashCode(), entity2.GetHashCode());
    }
    
    [Fact]
    public void GetHashCode_WhenNotEqual_ShouldNotBeSame()
    {
        var entity1 = new CustomEntity();
        var entity2 = new CustomEntity();

        Assert.NotEqual(entity1.GetHashCode(), entity2.GetHashCode());
    }
}