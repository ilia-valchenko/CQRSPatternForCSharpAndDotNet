using System;
using System.Threading.Tasks;
using CqrsFramework;
using Infrastructure.Interfaces;

namespace UseCases.Order.CheckOrder
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
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
}