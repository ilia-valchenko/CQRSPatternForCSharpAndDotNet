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
* test3

This is how the controller looks like in the beginning. A set of handlers was injected in the controller's constructor.

```csharp
// We don't need to pass all of the handlers in the constructor as we
// can have a lot of them. We can use ASP .NET Core injection method.
// See the second action as an example.
public OrderController(
	IRequestHandler<int, OrderDto> getOrderHandler,
	IRequestHandler<UpdateOrderCommand, Unit> updateOrderHandler)
{
    this.getOrderHandler = getOrderHandler;
    this.updateOrderHandler = updateOrderHandler;
}

[HttpGet("{id}")]
public async Task<OrderDto> Get(int id)
{
    return await this.getOrderHandler.HandleAsync(id);
}

[HttpPost("{id}")]
public async Task Update(int id, [FromBody]OrderDto dto, [FromServices] IRequestHandler<UpdateOrderCommand, int> updateOrderCommandHandler)
{
    var command = new UpdateOrderCommand
    {
        Id = id,
        Dto = dto
    };

    await updateOrderCommandHandler.HandleAsync(command);
}
```

Let's refactor our code by using `IHandlerDispatcher` and `IRequest<TResponse>`.
As you can see in the code below we don't create handlers manually. Now we have
the dispatcher which is responsible for it.

```csharp
public class GetOrderQuery : IRequest<OrderDto>
{
    public int Id { get; set; }
}

public OrderController(IHandlerDispatcher handlerDispatcher)
{
    this.handlerDispatcher = handlerDispatcher;
}

[HttpGet("{id}")]
public async Task<OrderDto> Get(int id)
{
    return await this.handlerDispatcher.SendAsync<OrderDto>(new GetOrderQuery { Id = id });
}
```