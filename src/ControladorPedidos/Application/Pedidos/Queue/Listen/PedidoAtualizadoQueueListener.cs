﻿using System.Text.Json;
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

            while (!stoppingToken.IsCancellationRequested)
            {
                var receiveMessageResponse = await amazonSQS.ReceiveMessageAsync(receiveMessageRequest, stoppingToken);
                if (receiveMessageResponse.Messages.Count == 0)
                {
                    logger.LogInformation("PedidoAtualizadoQueueListener - ReceiveMessageAsync - No messages");
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                    continue;
                }
                foreach (var message in receiveMessageResponse.Messages)
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
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Fatal - PedidoAtualizadoQueueListener - Error");
        }
    }
}
