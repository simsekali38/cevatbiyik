using CevatBiyik.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CevatBiyik.UnitTests;

public class InfrastructureDependencyInjectionTests
{
    [Fact]
    public void AddInfrastructure_AyniServiceCollectionDoner()
    {
        var services = new ServiceCollection();
        IConfiguration configuration = new ConfigurationBuilder().Build();

        IServiceCollection result = services.AddInfrastructure(configuration);

        Assert.Same(services, result);
    }

    [Fact]
    public void AddInfrastructure_NullArgumanIcinHataFirlatir()
    {
        var services = new ServiceCollection();
        IConfiguration configuration = new ConfigurationBuilder().Build();

        Assert.Throws<ArgumentNullException>(() => ((IServiceCollection)null!).AddInfrastructure(configuration));
        Assert.Throws<ArgumentNullException>(() => services.AddInfrastructure(null!));
    }
}
