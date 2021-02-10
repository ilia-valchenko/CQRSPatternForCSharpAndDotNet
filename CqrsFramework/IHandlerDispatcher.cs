using System.Threading.Tasks;

namespace CqrsFramework
{
    public interface IHandlerDispatcher
    {
        Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request);
    }
}