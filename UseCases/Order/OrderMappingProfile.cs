using AutoMapper;

namespace UseCases.Order
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<OrderDto, Domain.Order>().ReverseMap();
        }
    }
}