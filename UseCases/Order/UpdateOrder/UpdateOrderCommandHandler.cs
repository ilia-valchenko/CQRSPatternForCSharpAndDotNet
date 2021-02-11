using System.Threading.Tasks;
using AutoMapper;
using CqrsFramework;
using Infrastructure.Interfaces;

namespace UseCases.Order.UpdateOrder
{
    public class UpdateOrderCommandHandler : VoidRequestHandler<UpdateOrderCommand>
    {
        private readonly IDbContext dbContext;
        private readonly IMapper mapper;

        public UpdateOrderCommandHandler(IDbContext context, IMapper mapper)
        {
            this.dbContext = context;
            this.mapper = mapper;
        }

        protected override async Task HandleAsync(UpdateOrderCommand request)
        {
            var order = await this.dbContext.Orders.FindAsync(request.Id);
            this.mapper.Map(request.Dto, order);
            await this.dbContext.SaveChangesAsync();
        }
    }
}