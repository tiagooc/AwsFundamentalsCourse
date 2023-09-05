namespace Customers.Consumer.Handlers;

using MediatR;
using Messages;

public class CustomerUpdatedHandler : IRequestHandler<CustomerUpdated>
{
    private readonly ILogger<CustomerUpdatedHandler> _logger;

    public CustomerUpdatedHandler(ILogger<CustomerUpdatedHandler> logger)
    {
        _logger = logger;
    }

    public Task<Unit> Handle(CustomerUpdated request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received message with full name: {requestFullName}", request.FullName);
        
        return Unit.Task;
    }
}