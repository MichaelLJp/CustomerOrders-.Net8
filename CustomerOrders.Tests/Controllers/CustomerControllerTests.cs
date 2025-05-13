using Xunit;
using Moq;
using CustomerOrders.Application.Interfaces;
using CustomerOrders.Application.Dtos;
using CustomerOrders.Presentation.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;

namespace CustomerOrders.Tests.Controllers;

public class CustomerControllerTests
{
    private readonly Mock<ICustomerService> _customerServiceMock;
    private readonly CustomerController _controller;

    public CustomerControllerTests()
    {
        _customerServiceMock = new Mock<ICustomerService>();
        _controller = new CustomerController(_customerServiceMock.Object);
    }

    [Fact]
    public async Task GetCustomers_ShouldReturnOk_WhenCustomersExist()
    {
        var customers = new List<CustomerDto>
        {
            new CustomerDto { Id = 1, Name = "Alice" },
            new CustomerDto { Id = 2, Name = "Bob" }
        };

        _customerServiceMock.Setup(service => service.GetAllCustomersAsync()).ReturnsAsync(customers);

        var result = await _controller.GetCustomers();

        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(customers);
    }

    [Fact]
    public async Task GetCustomerById_ShouldReturnOk_WhenCustomerExists()
    {
        var customer = new CustomerDto { Id = 1, Name = "Alice" };

        _customerServiceMock.Setup(service => service.GetCustomerByIdAsync(1)).ReturnsAsync(customer);

        var result = await _controller.GetCustomerById(1);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(customer);
    }

    [Fact]
    public async Task GetCustomerById_ShouldReturnNotFound_WhenCustomerDoesNotExist()
    {
        _customerServiceMock.Setup(service => service.GetCustomerByIdAsync(99)).ThrowsAsync(new KeyNotFoundException("Customer with ID 99 not found."));

        var result = await _controller.GetCustomerById(99);

        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult.StatusCode.Should().Be(404);
        notFoundResult.Value.Should().Be("Customer with ID 99 not found.");
    }

    [Fact]
    public async Task CreateCustomer_ShouldReturnCreated_WhenCustomerIsValid()
    {
        var customerRequestDto = new CustomerRequestDto { Name = "Alice" };
        var createdCustomer = new CustomerDto { Id = 1, Name = "Alice" };

        _customerServiceMock.Setup(service => service.CreateCustomerAsync(customerRequestDto)).ReturnsAsync(createdCustomer);

        var result = await _controller.CreateCustomer(customerRequestDto);

        var createdResult = result.Result as CreatedAtActionResult;
        createdResult.Should().NotBeNull();
        createdResult.StatusCode.Should().Be(201);
        createdResult.Value.Should().BeEquivalentTo(createdCustomer);
    }

    [Fact]
    public async Task CreateCustomer_ShouldReturnBadRequest_WhenCreationFails()
    {
        var customerRequestDto = new CustomerRequestDto { Name = "Alice" };

        _customerServiceMock.Setup(service => service.CreateCustomerAsync(customerRequestDto))
            .ThrowsAsync(new InvalidOperationException("Invalid customer data."));

        var result = await _controller.CreateCustomer(customerRequestDto);

        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
        badRequestResult.Value.Should().Be("Invalid customer data.");
    }

    [Fact]
    public async Task UpdateCustomer_ShouldReturnOk_WhenCustomerIsUpdated()
    {
        var customerRequestDto = new CustomerRequestDto { Name = "UpdatedName" };
        var updatedCustomer = new CustomerDto { Id = 1, Name = "UpdatedName" };

        _customerServiceMock.Setup(service => service.UpdateCustomerAsync(1, customerRequestDto)).ReturnsAsync(updatedCustomer);

        var result = await _controller.UpdateCustomer(1, customerRequestDto);

        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(updatedCustomer);
    }

    [Fact]
    public async Task UpdateCustomer_ShouldReturnNotFound_WhenCustomerDoesNotExist()
    {
        var customerRequestDto = new CustomerRequestDto { Name = "UpdatedName" };

        _customerServiceMock.Setup(service => service.UpdateCustomerAsync(99, customerRequestDto))
            .ThrowsAsync(new KeyNotFoundException("Customer with ID 99 not found."));

        var result = await _controller.UpdateCustomer(99, customerRequestDto);

        var notFoundResult = result.Result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult.StatusCode.Should().Be(404);
        notFoundResult.Value.Should().Be("Customer with ID 99 not found.");
    }

    [Fact]
    public async Task DeleteCustomer_ShouldReturnOk_WhenCustomerIsDeleted()
    {
        _customerServiceMock.Setup(service => service.DeleteCustomerAsync(1)).Returns(Task.CompletedTask);

        var result = await _controller.DeleteCustomer(1);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(new { message = "Customer successfully deleted" });
    }

    [Fact]
    public async Task DeleteCustomer_ShouldReturnNotFound_WhenCustomerDoesNotExist()
    {
        _customerServiceMock.Setup(service => service.DeleteCustomerAsync(99))
            .ThrowsAsync(new KeyNotFoundException("Customer with ID 99 not found."));

        var result = await _controller.DeleteCustomer(99);

        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult.StatusCode.Should().Be(404);
        notFoundResult.Value.Should().Be("Customer with ID 99 not found.");
    }
}