namespace Customers.Api.Mapping;

using Contracts.Messages;
using Domain;

public static class DomainToMessageMapper
{
    public static CustomerCreated ToCustomerCreatedMessage(this Customer customer)
    {
        return new CustomerCreated
        {
            Email = customer.Email,
            Id = customer.Id,
            FullName = customer.FullName,
            DateOfBirth = customer.DateOfBirth,
            GitHubUsername = customer.GitHubUsername
        };
    }

    public static CustomerUpdated ToCustomerUpdatedMessage(this Customer customer)
    {
        return new CustomerUpdated
        {
            Email = customer.Email,
            Id = customer.Id,
            FullName = customer.FullName,
            DateOfBirth = customer.DateOfBirth,
            GitHubUsername = customer.GitHubUsername
        };
    }
}