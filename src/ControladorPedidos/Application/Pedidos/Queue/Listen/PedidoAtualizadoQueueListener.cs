using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using ControladorPedidos.Application.Pedidos.Commands;
using ControladorPedidos.Application.Pedidos.Queue.Models;
using ControladorPedidos.Infrastructure.Queue;
using MediatR;

namespace ControladorPedidos.Application.Pedidos.Queue.Listen;

public class PedidoAtualizadoQueueListener(
    ILogger<PedidoAtualizadoQueueListener> logger,
    IServiceProvider serviceProvider,
    IAmazonSQS amazonSQS
) : BackgroundService
{
    private readonly JsonSerializerOptions jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("PedidoAtualizadoQueueListener - Start");
        using var scope = serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        try
        {
            var queueUrlResponse = await amazonSQS.GetQueueUrlAsync(QueueConstants.PedidoAtualizado, stoppingToken);
            var receiveMessageRequest = new ReceiveMessageRequest
            {
                QueueUrl = queueUrlResponse.QueueUrl
            };

            var queueCozinhaUrl = await amazonSQS.GetQueueUrlAsync(QueueConstants.PedidoCozinha, stoppingToken);
            var receiveCozinhaRequest = new ReceiveMessageRequest
            {
                QueueUrl = queueCozinhaUrl.QueueUrl
            };

            while (!stoppingToken.IsCancellationRequested)
            {
                var receiveMessageAtualizacaoResponse = await amazonSQS.ReceiveMessageAsync(receiveMessageRequest, stoppingToken);
                var receiveMessageCozinhaResponse = await amazonSQS.ReceiveMessageAsync(receiveCozinhaRequest, stoppingToken);

                if (receiveMessageAtualizacaoResponse.Messages.Count == 0 && receiveMessageCozinhaResponse.Messages.Count == 0)
                {
                    logger.LogInformation("PedidoAtualizadoQueueListener/PedidoCozinhaQueueListener - ReceiveMessageAsync - No messages");
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                    continue;
                }

                foreach (var message in receiveMessageAtualizacaoResponse.Messages)
                {
                    try
                    {
                        logger.LogInformation("PedidoAtualizadoQueueListener - ReceiveMessageAsync - Start");
                        PedidoQueue pedidoQueue = JsonSerializer.Deserialize<PedidoQueue>(message.Body, jsonSerializerOptions)!;
                        AtualizarPedidoCommand command = (AtualizarPedidoCommand)pedidoQueue;
                        await mediator.Send(command, stoppingToken);

                        logger.LogInformation("PedidoAtualizadoQueueListener - ReceiveMessageAsync - End");

                        await amazonSQS.DeleteMessageAsync(queueUrlResponse.QueueUrl, message.ReceiptHandle, stoppingToken);
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, "PedidoAtualizadoQueueListener - Error");
                    }
                }

                foreach (var message in receiveMessageCozinhaResponse.Messages)
                {
                    try
                    {
                        logger.LogInformation("PedidoCozinhaQueueListener - ReceiveMessageAsync - Start");
                        PedidoCozinhaQueue pedidoQueue = JsonSerializer.Deserialize<PedidoCozinhaQueue>(message.Body, jsonSerializerOptions)!;
                        AtualizarPedidoCommand command = (AtualizarPedidoCommand)pedidoQueue;
                        await mediator.Send(command, stoppingToken);

                        logger.LogInformation("PedidoCozinhaQueueListener - ReceiveMessageAsync - End");

                        await amazonSQS.DeleteMessageAsync(queueUrlResponse.QueueUrl, message.ReceiptHandle, stoppingToken);
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, "PedidoCozinhaQueueListener - Error");
                    }
                }


            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Fatal - PedidoAtualizadoQueueListener - Error");
        }
    }
}
