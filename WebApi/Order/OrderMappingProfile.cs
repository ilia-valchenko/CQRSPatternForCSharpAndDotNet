using AutoMapper;

namespace WebApi.Order
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<OrderDto, Domain.Order>().ReverseMap();
        }
    }
}