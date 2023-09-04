namespace Customers.Api.Messaging;

using Amazon.SQS.Model;

public interface ISqsMessenger
{
    Task<SendMessageResponse> SendMessageAsync<T>(T message);
}