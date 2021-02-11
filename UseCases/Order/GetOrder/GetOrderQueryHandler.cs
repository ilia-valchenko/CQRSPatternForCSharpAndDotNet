using System;
using System.Threading.Tasks;
using AutoMapper;
using CqrsFramework;
using Infrastructure.Interfaces;

namespace UseCases.Order.GetOrder
{
    public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, OrderDto>
    {
        private readonly IDbContext dbContext;
        private readonly ICurrentUserService currentUserService;
        private readonly IMapper mapper;

        public GetOrderQueryHandler(IDbContext context, ICurrentUserService currentUserService, IMapper mapper)
        {
            this.dbContext = context;
            this.currentUserService = currentUserService;
            this.mapper = mapper;
        }

        public async Task<OrderDto> HandleAsync(GetOrderQuery request)
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

            return this.mapper.Map<OrderDto>(order);
        }
    }
}