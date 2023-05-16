using Orleans.TestingHost;

namespace Conduit.Test.Integration;

public class ClusterFixture : IDisposable
{
    public ClusterFixture()
    {
        var builder = new TestClusterBuilder();
        builder.AddSiloBuilderConfigurator<TestSiloConfigurations>();
        Cluster = builder.Build();
        Cluster.Deploy();
    }

    public void Dispose() => Cluster.StopAllSilos();

    public TestCluster Cluster { get; private set; }

    public class TestSiloConfigurations : ISiloConfigurator
    {
        public void Configure(ISiloBuilder siloBuilder)
        {
            siloBuilder.ConfigureServices(services =>
            {
                _ = siloBuilder.UseLocalhostClustering();
                _ = siloBuilder.AddMemoryGrainStorage("conduit");
            });
        }
    }
}