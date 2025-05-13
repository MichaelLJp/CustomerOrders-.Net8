using AutoMapper;
using CustomerOrders.Application.Dtos;
using CustomerOrders.Application.Interfaces;
using CustomerOrders.Domain.Entities;
using CustomerOrders.Domain.Interfaces;

namespace CustomerOrders.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly IRepository<Customer> _customerRepository;
    private readonly IMapper _mapper;

    public CustomerService(IRepository<Customer> repository, IMapper mapper)
    {
        _customerRepository = repository;
        _mapper = mapper;
    }


    public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync()
    {
        var customers = await _customerRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<CustomerDto>>(customers);
    }

    public async Task<CustomerDto> GetCustomerByIdAsync(int id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null)
        {
            throw new KeyNotFoundException($"Customer with ID {id} not found.");
        }
        return _mapper.Map<CustomerDto>(customer);
    }

    public async Task<CustomerDto> CreateCustomerAsync(CustomerRequestDto customerRequestDto)
    {
        var customer = _mapper.Map<Customer>(customerRequestDto);
        await _customerRepository.CreateAsync(customer);
        return _mapper.Map<CustomerDto>(customer);
    }

    public async Task<CustomerDto> UpdateCustomerAsync(int id, CustomerRequestDto customerRequestDto)
    {
        var existingCustomer = await _customerRepository.GetByIdAsync(id);
        if (existingCustomer == null)
        {
            throw new KeyNotFoundException($"Customer with ID {id} not found.");
        }
        _mapper.Map(customerRequestDto, existingCustomer);
        await _customerRepository.UpdateAsync(existingCustomer);
        return _mapper.Map<CustomerDto>(existingCustomer);
    }

    public async Task DeleteCustomerAsync(int id)
    {
        var existingCustomer = await _customerRepository.GetByIdAsync(id);
        if (existingCustomer == null)
        {
            throw new KeyNotFoundException($"Customer with ID {id} not found.");
        }
        await _customerRepository.DeleteAsync(id);
    }
}
