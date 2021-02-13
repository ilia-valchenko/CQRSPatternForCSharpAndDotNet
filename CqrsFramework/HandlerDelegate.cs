using System.Threading.Tasks;

namespace CqrsFramework
{
    // This is the same implementation as ASP .NET Core has.
    public delegate Task<TResponse> HandlerDelegate<TResponse>();
}