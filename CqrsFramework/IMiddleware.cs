using System.Threading.Tasks;

namespace CqrsFramework
{
    public interface IMiddleware<TRequest, TResponse>
    {
        // We implemented it in a different way. We don't put the next part of
        // the pipeline in the constructor. We pass the second argument to the
        // HandleAsync method.
        Task<TResponse> HandleAsync(TRequest request, HandlerDelegate<TResponse> next);
    }
}