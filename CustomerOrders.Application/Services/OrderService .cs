using AutoMapper;
using CustomerOrders.Application.Dtos;
using CustomerOrders.Application.Interfaces;
using CustomerOrders.Domain.Entities;
using CustomerOrders.Domain.Interfaces;

namespace CustomerOrders.Application.Services
{
    public class OrderService:IOrderService
    {

        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IMapper _mapper;

        public OrderService(IRepository<Order> repository, IRepository<Customer> customerRepository, IMapper mapper)
        {
            _orderRepository = repository;
            _customerRepository = customerRepository;
            _mapper = mapper;
        }


        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<OrderDto> GetOrderByIdAsync(int id)
        {
            var Order = await _orderRepository.GetByIdAsync(id);
            if (Order == null)
            {
                throw new KeyNotFoundException($"Order with ID {id} not found.");
            }
            return _mapper.Map<OrderDto>(Order);
        }

        public async Task<OrderDto> CreateOrderAsync(OrderRequestDto orderRequestDto)
        {
            var customerExists = await _customerRepository.GetByIdAsync(orderRequestDto.CustomerId);
            if (customerExists == null)
            {
                throw new InvalidOperationException("The specified customer does not exist, please verify the entered CustomerId.");
            }
            var order = _mapper.Map<Order>(orderRequestDto);
            await _orderRepository.CreateAsync(order);
            return _mapper.Map<OrderDto>(order);
        }

      public async Task<OrderDto> UpdateOrderAsync(int id, OrderRequestDto orderRequestDto)
        {
            var existingOrder = await _orderRepository.GetByIdAsync(id);
            if (existingOrder == null)
            {
                throw new KeyNotFoundException($"Order with ID {id} not found.");
            }

            var customerExists = await _customerRepository.GetByIdAsync(orderRequestDto.CustomerId);
            if (customerExists == null)
            {
                throw new InvalidOperationException($"Customer with ID {orderRequestDto.CustomerId} does not exist. Cannot assign a non-existent customer.");
            }
            _mapper.Map(orderRequestDto, existingOrder);
            await _orderRepository.UpdateAsync(existingOrder);
            return _mapper.Map<OrderDto>(existingOrder);
        }

        public async Task DeleteOrderAsync(int id)
        {
            var existingOrder = await _orderRepository.GetByIdAsync(id);
            if (existingOrder == null)
            {
                throw new KeyNotFoundException($"Order with ID {id} not found.");
            }
            await _orderRepository.DeleteAsync(id);
        }
    }
}
