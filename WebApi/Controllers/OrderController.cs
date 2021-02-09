using System;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebApi.Order;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IDbContext dbContext;
        private readonly ICurrentUserService currentUserService;
        private readonly IMapper mapper;

        public OrderController(IDbContext dbContext, ICurrentUserService currentUserService, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.currentUserService = currentUserService;
            this.mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<OrderDto> Get(int id)
        {
            var order = await this.dbContext.Orders.FindAsync(id);

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

        public async Task Update(int id, [FromBody]OrderDto dto)
        {
            var order = await this.dbContext.Orders.FindAsync(id);

            if (order == null)
            {
                throw new Exception("Not found");
            }

            if (order.UserEmail != this.currentUserService.Email)
            {
                throw new Exception("Forbidden");
            }

            this.mapper.Map(dto, order);
            await this.dbContext.SaveChangesAsync();
        }
    }
}