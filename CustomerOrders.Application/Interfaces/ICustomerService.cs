using CustomerOrders.Application.Dtos;

namespace CustomerOrders.Application.Interfaces;

public interface ICustomerService
{
    Task<IEnumerable<CustomerDto>> GetAllCustomersAsync();
    Task<CustomerDto> GetCustomerByIdAsync(int id);
    Task<CustomerDto> CreateCustomerAsync(CustomerRequestDto createCustomerDto);
    Task<CustomerDto> UpdateCustomerAsync(int id, CustomerRequestDto customerRequestDto);
    Task DeleteCustomerAsync(int id);
}
