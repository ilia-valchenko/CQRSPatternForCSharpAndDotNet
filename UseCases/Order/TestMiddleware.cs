using System;
using System.Threading.Tasks;
using CqrsFramework;

namespace UseCases.Order
{
    public class TestMiddleware<TRequest, TResponse> : IMiddleware<TRequest, TResponse>
    {
        public Task<TResponse> HandleAsync(TRequest request, HandlerDelegate<TResponse> next)
        {
            throw new NotImplementedException();
        }
    }
}