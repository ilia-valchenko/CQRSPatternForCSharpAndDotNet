namespace CqrsFramework
{
    public interface IVoidRequestHandler<TRequest> : IRequestHandler<TRequest, Unit>
    {
    }
}