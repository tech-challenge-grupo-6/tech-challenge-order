namespace ControladorPedidos.Infrastructure.Queue;

public interface IQueueService
{
    Task CreateQueueAsync(string queueName);
    Task SendMessageAsync(string queueName, string message);
    Task ListenToQueueAsync(string queueName, Action<string> onMessageReceived, CancellationToken cancellationToken);
}
