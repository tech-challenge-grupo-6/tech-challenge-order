using DotNet.Testcontainers.Builders;
using TechTalk.SpecFlow;
using Testcontainers.MsSql;
using Testcontainers.Redis;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace ControladorPedidos.BDD.Tests.Hooks;

[Binding]
public class Hooks
{
    private static MsSqlContainer? _mySqlContainer;
    private static RedisContainer? _redisContainer;

    [BeforeTestRun]
    public static async Task BeforeTestRun()
    {
        _mySqlContainer = new MsSqlBuilder()
            .WithImage("mysql:latest")
            .WithEnvironment("MYSQL_ROOT_PASSWORD", "P@ssWord")
            .WithEnvironment("MYSQL_DATABASE", "controlador_pedidos")
            .WithPortBinding(3306)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(3306))
            .Build();
        await _mySqlContainer.StartAsync();

        _redisContainer = new RedisBuilder()
            .WithImage("redis:7.0")
            .WithPortBinding(6379)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(6379))
            .Build();
        await _redisContainer.StartAsync();
    }

    [AfterTestRun]
    public static async Task AfterTestRun()
    {
        if (_mySqlContainer != null)
        {
            await _mySqlContainer.StopAsync();
        }

        if (_redisContainer != null)
        {
            await _redisContainer.StopAsync();
        }
    }
}
