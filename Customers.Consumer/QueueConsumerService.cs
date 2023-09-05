namespace Customers.Consumer;

using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using Api.Messaging;
using MediatR;
using Messages;
using Microsoft.Extensions.Options;

public class QueueConsumerService : BackgroundService
{
    private readonly IAmazonSQS _sqs;
    private readonly IMediator _mediator;
    private readonly IOptions<QueueSettings> _queueSettings;
    private readonly ILogger<QueueConsumerService> _logger;

    public QueueConsumerService(
        IAmazonSQS sqs,
        IMediator mediator,
        IOptions<QueueSettings> queueSettings,
        ILogger<QueueConsumerService> queueConsumerService)
    {
        _sqs = sqs;
        _mediator = mediator;
        _queueSettings = queueSettings;
        _logger = queueConsumerService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var queueUrlResponse = await _sqs.GetQueueUrlAsync(_queueSettings.Value.Name, stoppingToken);

        var receiveMessageRequest = new ReceiveMessageRequest
        {
            QueueUrl = queueUrlResponse.QueueUrl,
            AttributeNames = new List<string> { "All" },
            MessageAttributeNames = new List<string> { "All" },
            MaxNumberOfMessages = 1
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            var response = await _sqs.ReceiveMessageAsync(receiveMessageRequest, stoppingToken);

            foreach (var message in response.Messages)
            {
                var messageType = message.MessageAttributes["MessageType"]?.StringValue;
                var type = Type.GetType($"Customers.Consumer.Messages.{messageType}");
                if (type is null)
                {
                    _logger.LogWarning("Unknown message type of type: {messageType}", messageType);
                    continue;
                }

                var typedMessage = (ISqsMessage)JsonSerializer.Deserialize(message.Body, type)!;

                try
                {
                    await _mediator.Send(typedMessage, stoppingToken);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);

                    _logger.LogError(e, "Message failed during processing");
                    continue;
                }

                await _sqs.DeleteMessageAsync(queueUrlResponse.QueueUrl, message.ReceiptHandle, stoppingToken);
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}