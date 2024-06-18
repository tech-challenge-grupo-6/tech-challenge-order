namespace ControladorPedidos.Infrastructure.Queue;

public interface IQueueStartup
{
    Task CreateQueuesAsync();
}
