using System.Text.Json;
using ControladorPedidos.Application.Pedidos.Commands;
using ControladorPedidos.Application.Pedidos.Queue.Models;
using ControladorPedidos.Infrastructure.Queue;
using MediatR;

namespace ControladorPedidos.Application.Pedidos.Queue.Listen;

public class PedidoAtualizadoQueueListener(
    ILogger<PedidoAtualizadoQueueListener> logger,
    IMediator mediator,
    IQueueService queueService,
    JsonSerializerOptions jsonSerializerOptions
) : IHostedService, IDisposable
{
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("PedidoAtualizadoQueueListener - Start");

        await queueService.ListenToQueueAsync(QueueConstants.PedidoAtualizado, async (message) =>
        {
            try
            {
                logger.LogInformation("PedidoAtualizadoQueueListener - ReceiveMessageAsync - Start");

                PedidoQueue pedidoQueue = JsonSerializer.Deserialize<PedidoQueue>(message, jsonSerializerOptions)!;
                AtualizarPedidoCommand command = (AtualizarPedidoCommand)pedidoQueue;

                await mediator.Send(command, cancellationToken);

                logger.LogInformation("PedidoAtualizadoQueueListener - ReceiveMessageAsync - End");
            }
            catch (Exception e)
            {
                logger.LogError(e, "PedidoAtualizadoQueueListener - ReceiveMessageAsync - Error");
            }
        }, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("PedidoAtualizadoQueueListener - Stop");
        return Task.CompletedTask;
    }
}
