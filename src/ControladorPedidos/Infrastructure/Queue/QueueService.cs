
using Amazon.SQS;
using Amazon.SQS.Model;

namespace ControladorPedidos.Infrastructure.Queue;

public class QueueService(IAmazonSQS amazonSQS) : IQueueService
{
    public async Task CreateQueueAsync(string queueName)
    {
        await amazonSQS.CreateQueueAsync(queueName);
    }

    public async Task ListenToQueueAsync(string queueName, Action<string> onMessageReceived, CancellationToken cancellationToken)
    {
        var queueUrlResponse = await amazonSQS.GetQueueUrlAsync(queueName, cancellationToken);
        var receiveMessageRequest = new ReceiveMessageRequest
        {
            QueueUrl = queueUrlResponse.QueueUrl
        };

        while (!cancellationToken.IsCancellationRequested)
        {
            var receiveMessageResponse = await amazonSQS.ReceiveMessageAsync(receiveMessageRequest, cancellationToken);
            foreach (var message in receiveMessageResponse.Messages)
            {
                onMessageReceived(message.Body);
                await amazonSQS.DeleteMessageAsync(queueUrlResponse.QueueUrl, message.ReceiptHandle, cancellationToken);
            }
        }
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
