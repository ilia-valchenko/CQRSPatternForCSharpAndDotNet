# CQRS pattern for C# and .NET

![alt text](https://habrastorage.org/getpro/habr/post_images/d4d/f93/26b/d4df9326bc3d3794857462bae9abe30e.svg)

Client sends a command to a back-end. We have two separate storages for reading and writing. They can be non-relational databases. These storages sync from time to time. But I have never seen such ideal projects. Most often I see projects which use only one database. They usualy use Command, BLL and ORM for writing operations.

Each method or use case will be transformed to a command or a query.

```cs
public class AccountRepository : IRepository<Account>
{
    // GetActiveAccountsQuery
    public IEnumerable<Account> GetActiveAccounts()
    {
        // ...
    }

    // ChangeAccountAddressCommand
    public void ChangeAccountAddress(int id, string newAddress)
    {
        // ...
    }

    // GetPremiumAccountsByManagerQuery
    public IEnumerable<Account> GetPremiumAccountsByManager()
    {
        // ...
    }
}
```

```csharp
public class PayOrderCommand
{
    public int OrderId { get; set; }
}

public class PayOrderCommandHandler : ICommandHandler<PayOrderCommand>
{
    public void Handle(PayOrderCommand command)
    {
        // ...
    }
}
```

## What are benefits of all these modifications if we don't have a separate database and don't have a separate read-model?
Architecture benefits:
* test1
* test2