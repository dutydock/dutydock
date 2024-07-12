using DutyDock.Domain.Common.Services;
using Xunit;
using Xunit.Abstractions;

namespace DutyDock.Domain.Tests.Unit.Common.Services;

public class IdentityProviderTests
{
    private readonly ITestOutputHelper _output;

    public IdentityProviderTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void New_WhenInvoked_ShouldBeValid()
    {
        for (var i = 0; i < 10000; i++)
        {
            var id = IdentityProvider.New();
            _output.WriteLine(id);
            Assert.True(IdentityProvider.IsValid(id));
        }
    }
}