using AutoMapper;
using CustomerOrders.Domain.Entities;
using CustomerOrders.Application.Dtos;

namespace CustomerOrders.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Customer, CustomerDto>().ReverseMap();
            CreateMap<Customer, CustomerRequestDto>().ReverseMap();
            CreateMap<Order, OrderDto>().ReverseMap();
            CreateMap<Order, OrderRequestDto>().ReverseMap();

        }
    }
}