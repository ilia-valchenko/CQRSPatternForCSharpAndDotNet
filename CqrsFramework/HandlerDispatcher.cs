using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace CqrsFramework
{
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
}