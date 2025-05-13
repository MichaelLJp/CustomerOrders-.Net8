using Xunit;
using Moq;
using CustomerOrders.Application.Services;
using CustomerOrders.Application.Dtos;
using CustomerOrders.Domain.Entities;
using CustomerOrders.Domain.Interfaces;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;

namespace CustomerOrders.Tests.Services;
public class CustomerServiceTests
{
    private readonly Mock<IRepository<Customer>> _customerRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CustomerService _customerService;

    public CustomerServiceTests()
    {
        _customerRepositoryMock = new Mock<IRepository<Customer>>();
        _mapperMock = new Mock<IMapper>();
        _customerService = new CustomerService(_customerRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetAllCustomersAsync_ShouldReturnCustomers_WhenCustomersExist()
    {
        var customers = new List<Customer>
        {
            new Customer { Id = 1, Name = "Alice" },
            new Customer { Id = 2, Name = "Bob" }
        };

        _customerRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(customers);
        _mapperMock.Setup(map => map.Map<IEnumerable<CustomerDto>>(customers)).Returns(new List<CustomerDto>
        {
            new CustomerDto { Id = 1, Name = "Alice" },
            new CustomerDto { Id = 2, Name = "Bob" }
        });

        var result = await _customerService.GetAllCustomersAsync();

        result.Should().NotBeNullOrEmpty();
        result.Should().HaveCount(2);
        result.First().Id.Should().Be(1);
        result.Last().Id.Should().Be(2);
    }

    [Fact]
    public async Task GetCustomerByIdAsync_ShouldThrowException_WhenCustomerDoesNotExist()
    {
        _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(99)).ReturnsAsync((Customer?)null);

        var act = async () => await _customerService.GetCustomerByIdAsync(99);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Customer with ID 99 not found.");
    }

    [Fact]
    public async Task GetCustomerByIdAsync_ShouldReturnCustomer_WhenExists()
    {
        var customer = new Customer { Id = 1, Name = "Alice" };
        _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(customer);
        _mapperMock.Setup(map => map.Map<CustomerDto>(customer)).Returns(new CustomerDto { Id = 1, Name = "Alice" });

        var result = await _customerService.GetCustomerByIdAsync(1);

        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Name.Should().Be("Alice");
    }

    [Fact]
    public async Task CreateCustomerAsync_ShouldCreateCustomer()
    {
        var customerRequestDto = new CustomerRequestDto { Name = "Alice" };
        var customer = new Customer { Id = 1, Name = "Alice" };

        _customerRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Customer>())).Returns(Task.CompletedTask);
        _mapperMock.Setup(map => map.Map<Customer>(customerRequestDto)).Returns(customer);
        _mapperMock.Setup(map => map.Map<CustomerDto>(customer)).Returns(new CustomerDto { Id = 1, Name = "Alice" });

        var result = await _customerService.CreateCustomerAsync(customerRequestDto);

        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Name.Should().Be("Alice");
    }

    [Fact]
    public async Task UpdateCustomerAsync_ShouldUpdateCustomer_WhenCustomerExists()
    {
        var customerRequestDto = new CustomerRequestDto { Name = "UpdatedName" };
        var existingCustomer = new Customer { Id = 1, Name = "Alice" };

        _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingCustomer);
        _mapperMock.Setup(map => map.Map(customerRequestDto, existingCustomer))
            .Callback<CustomerRequestDto, Customer>((dto, customer) => customer.Name = dto.Name);

        await _customerService.UpdateCustomerAsync(1, customerRequestDto);

        existingCustomer.Name.Should().Be("UpdatedName");
    }

    [Fact]
    public async Task UpdateCustomerAsync_ShouldThrowException_WhenCustomerDoesNotExist()
    {
        var customerRequestDto = new CustomerRequestDto { Name = "UpdatedName" };

        _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(99)).ReturnsAsync((Customer?)null);

        var act = async () => await _customerService.UpdateCustomerAsync(99, customerRequestDto);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Customer with ID 99 not found.");
    }

   

    [Fact]
    public async Task DeleteCustomerAsync_ShouldDeleteCustomer_WhenCustomerExists()
    {
        var existingCustomer = new Customer { Id = 1, Name = "Alice" };

        _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingCustomer);
        _customerRepositoryMock.Setup(repo => repo.DeleteAsync(1)).ReturnsAsync(true);

        await _customerService.DeleteCustomerAsync(1);

        _customerRepositoryMock.Verify(repo => repo.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteCustomerAsync_ShouldThrowException_WhenCustomerDoesNotExist()
    {
        _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(99)).ReturnsAsync((Customer?)null);

        var act = async () => await _customerService.DeleteCustomerAsync(99);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Customer with ID 99 not found.");
    }
}