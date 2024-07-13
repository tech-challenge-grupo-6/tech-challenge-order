using System.Text.Json;
using ControladorPedidos.Application.Pedidos.Models;
using ControladorPedidos.Application.Pedidos.Queue.Models;
using ControladorPedidos.Application.Pedidos.Queue.Send;
using ControladorPedidos.Infrastructure.Queue;
using Microsoft.Extensions.Logging;

namespace ControladorPedido.Tests.Application.Pedidos.Queue;

public class PedidoCriadoQueueSendMessageHandlerTests
{
    [Fact]
    public async Task Handle_ShouldSendMessageToQueue()
    {
        // Arrange
        var logger = Substitute.For<ILogger<PedidoCriadoQueueSendMessageHandler>>();
        var queueService = Substitute.For<IQueueService>();
        var jsonSerializerOptions = new JsonSerializerOptions();
        var pedidoQueue = new PedidoQueue("test", Status.Criado, "test", [], 0.0, false);
        var notification = new PedidoCriadoQueueSendMessage(pedidoQueue);
        var handler = new PedidoCriadoQueueSendMessageHandler(logger, queueService, jsonSerializerOptions);

        // Act
        await handler.Handle(notification, CancellationToken.None);

        // Assert
        await queueService.Received(1).SendMessageAsync(
            QueueConstants.PedidoCriado,
            JsonSerializer.Serialize(pedidoQueue, jsonSerializerOptions));
    }
}
