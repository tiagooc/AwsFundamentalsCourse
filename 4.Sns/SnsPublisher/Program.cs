using System.Text.Json;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using SnsPublisher;

var snsClient = new AmazonSimpleNotificationServiceClient();

var topic = await snsClient.FindTopicAsync("customers");

var customerCreated = new CustomerCreated
{
    Email = "qwerty@asdas.com",
    Id = Guid.NewGuid(),
    FullName = "Tiago Costa",
    DateOfBirth = new DateTime(1991, 1, 1),
    GitHubUsername = "tiagooc"
};

var publishRequest = new PublishRequest
{
    TopicArn = topic.TopicArn,
    Message = JsonSerializer.Serialize(customerCreated),
    MessageAttributes = new Dictionary<string, MessageAttributeValue>
    {
        {
            "MessageType", new MessageAttributeValue
            {
                DataType = "String",
                StringValue = nameof(CustomerCreated)
            }
        }
    }
};

await snsClient.PublishAsync(publishRequest);