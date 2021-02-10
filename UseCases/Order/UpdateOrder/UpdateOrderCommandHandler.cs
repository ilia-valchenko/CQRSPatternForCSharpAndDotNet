using System;
using System.Threading.Tasks;
using AutoMapper;
using CqrsFramework;
using Infrastructure.Interfaces;

namespace UseCases.Order.UpdateOrder
{
    public class UpdateOrderCommandHandler : VoidRequestHandler<UpdateOrderCommand>
    {
        private readonly IDbContext dbContext;
        private readonly ICurrentUserService currentUserService;
        private readonly IMapper mapper;

        public UpdateOrderCommandHandler(IDbContext context, ICurrentUserService currentUserService, IMapper mapper)
        {
            this.dbContext = context;
            this.currentUserService = currentUserService;
            this.mapper = mapper;
        }

        protected override async Task HandleAsync(UpdateOrderCommand request)
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

            this.mapper.Map(request.Dto, order);
            await this.dbContext.SaveChangesAsync();
        }
    }
}