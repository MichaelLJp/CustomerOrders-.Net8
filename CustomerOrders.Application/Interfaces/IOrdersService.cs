using CustomerOrders.Application.Dtos;

namespace CustomerOrders.Application.Interfaces;

public interface IOrderService
{
    Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
    Task<OrderDto> GetOrderByIdAsync(int id);
    Task<OrderDto> CreateOrderAsync(OrderRequestDto createOrderDto);
    Task<OrderDto> UpdateOrderAsync(int id, OrderRequestDto orderRequestDto);
    Task DeleteOrderAsync(int id);
}
