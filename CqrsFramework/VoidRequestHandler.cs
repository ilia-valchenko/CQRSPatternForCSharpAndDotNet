using System.Threading.Tasks;

namespace CqrsFramework
{
    public abstract class VoidRequestHandler<TRequest> : IVoidRequestHandler<TRequest>
    {
        async Task<Unit> IRequestHandler<TRequest, Unit>.HandleAsync(TRequest request)
        {
            await HandleAsync(request);

            return Unit.value;
        }

        protected abstract Task HandleAsync(TRequest request);
    }
}