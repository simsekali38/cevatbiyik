using CevatBiyik.Application;
using Microsoft.Extensions.DependencyInjection;

namespace CevatBiyik.UnitTests;

public class ApplicationDependencyInjectionTests
{
    [Fact]
    public void AddApplication_AyniServiceCollectionDoner()
    {
        var services = new ServiceCollection();

        var result = services.AddApplication();

        Assert.Same(services, result);
    }

    [Fact]
    public void AddApplication_NullServiceCollectionIcinHataFirlatir()
    {
        IServiceCollection? services = null;

        Assert.Throws<ArgumentNullException>(() => services!.AddApplication());
    }
}
