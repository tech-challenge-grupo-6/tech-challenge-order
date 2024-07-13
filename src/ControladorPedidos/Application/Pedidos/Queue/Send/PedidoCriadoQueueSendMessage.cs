using System.Text.Json;
using ControladorPedidos.Application.Pedidos.Queue.Models;
using ControladorPedidos.Infrastructure.Queue;
using MediatR;

namespace ControladorPedidos.Application.Pedidos.Queue.Send;

public record PedidoCriadoQueueSendMessage(PedidoQueue PedidoQueue) : INotification;

public class PedidoCriadoQueueSendMessageHandler(
    ILogger<PedidoCriadoQueueSendMessageHandler> logger,
    IQueueService queueService,
    JsonSerializerOptions jsonSerializerOptions) : INotificationHandler<PedidoCriadoQueueSendMessage>
{

    public async Task Handle(PedidoCriadoQueueSendMessage notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("PedidoCriadoQueueSendMessageHandler - Handle - Start");

        await queueService.SendMessageAsync(QueueConstants.PedidoCriado, JsonSerializer.Serialize(notification.PedidoQueue, jsonSerializerOptions));

        logger.LogInformation("PedidoCriadoQueueSendMessageHandler - Handle - End");
    }
}
