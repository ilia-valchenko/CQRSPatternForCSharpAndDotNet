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

Now we will add a middleware. We will create a pipeline which will handle our requests.

```csharp
public delegate Task<TResponse> HandlerDelegate<TResponse>();

public interface IMiddleware<TRequest, TResponse>
{
    // We implemented it in a different way. We don't put the next part of
    // the pipeline in the constructor. We pass the second argument to the
    // HandleAsync method.
    Task<TResponse> HandleAsync(TRequest request, HandlerDelegate<TResponse> next);
}

public interface ICheckOrderRequest
{
    public int Id { get; set; }
}

public class CheckOrderMiddleware<TRequest, TResponse> : IMiddleware<TRequest, TResponse>
    where TRequest : ICheckOrderRequest
{
    private readonly IDbContext dbContext;
    private readonly ICurrentUserService currentUserService;

    public CheckOrderMiddleware(IDbContext dbContext, ICurrentUserService currentUserService)
    {
        this.dbContext = dbContext;
        this.currentUserService = currentUserService;
    }

    public async Task<TResponse> HandleAsync(TRequest request, HandlerDelegate<TResponse> next)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var order = await this.dbContext.Orders.FindAsync(request.Id);

        if (order == null)
        {
            throw new Exception("Not found");
        }

        if (order.UserEmail != this.currentUserService.Email)
        {
            throw new Exception("Forbidden");
        }

        return await next();
    }
}

public class HandlerDispatcher : IHandlerDispatcher
{
    private readonly IServiceProvider serviceProvider;

    public HandlerDispatcher(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var method = this.GetType()
            .GetMethod("HandleAsync", BindingFlags.NonPublic | BindingFlags.Instance)
            .MakeGenericMethod(request.GetType(), typeof(TResponse));

        var result = method.Invoke(this, new[] { request });

        return (Task<TResponse>) result;
    }

    protected Task<TResponse> HandleAsync<TRequest, TResponse>(TRequest request)
    {
        var handler = this.serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();
        var middlewares = this.serviceProvider.GetServices<IMiddleware<TRequest, TResponse>>();
        HandlerDelegate<TResponse> handlerDelegate = () => handler.HandleAsync(request);

        // Now we want to call all middlewares.
        // We transform the collection to a single delefate.
        var resultDelegate = middlewares.Aggregate(handlerDelegate, (next, middleware) => () => middleware.HandleAsync(request, next));

        return resultDelegate();
    }
}

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly IHandlerDispatcher handlerDispatcher;

    public OrderController(IHandlerDispatcher handlerDispatcher)
    {
        this.handlerDispatcher = handlerDispatcher;
    }

    [HttpGet("{id}")]
    public async Task<OrderDto> Get(int id)
    {
        return await this.handlerDispatcher.SendAsync<OrderDto>(new GetOrderQuery { Id = id });
    }
}
```

Summary: We used 10 infrastructure elements to implement CQRS pattern (`CqrsFramework` project consists of 10 classes).