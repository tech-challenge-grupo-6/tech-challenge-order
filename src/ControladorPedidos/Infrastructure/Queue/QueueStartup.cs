
namespace ControladorPedidos.Infrastructure.Queue;

public class QueueStartup(ILogger<QueueStartup> logger, IQueueService queueService) : IQueueStartup
{
    public async Task CreateQueuesAsync()
    {
        try
        {
            await queueService.CreateQueueAsync(QueueConstants.PedidoCriado);
            await queueService.CreateQueueAsync(QueueConstants.PedidoAtualizado);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating queues");
        }
    }
}
