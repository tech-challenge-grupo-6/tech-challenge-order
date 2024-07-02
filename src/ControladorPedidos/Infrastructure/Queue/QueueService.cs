
using Amazon.SQS;
using Amazon.SQS.Model;

namespace ControladorPedidos.Infrastructure.Queue;

public class QueueService(IAmazonSQS amazonSQS) : IQueueService
{
    public async Task CreateQueueAsync(string queueName)
    {
        await amazonSQS.CreateQueueAsync(queueName);
    }

    public async Task SendMessageAsync(string queueName, string message)
    {
        var queueUrlResponse = await amazonSQS.GetQueueUrlAsync(queueName);
        var sendMessageRequest = new SendMessageRequest
        {
            QueueUrl = queueUrlResponse.QueueUrl,
            MessageBody = message
        };
        await amazonSQS.SendMessageAsync(sendMessageRequest);
    }
}
