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

public class OrderControllerTests
{
    private readonly Mock<IOrderService> _orderServiceMock;
    private readonly OrderController _controller;

    public OrderControllerTests()
    {
        _orderServiceMock = new Mock<IOrderService>();
        _controller = new OrderController(_orderServiceMock.Object);
    }

    [Fact]
    public async Task GetAllOrders_ShouldReturnOk_WhenOrdersExist()
    {
        var orders = new List<OrderDto>
        {
            new OrderDto { Id = 1, CustomerId = 3 },
            new OrderDto { Id = 2, CustomerId = 4 }
        };

        _orderServiceMock.Setup(service => service.GetAllOrdersAsync()).ReturnsAsync(orders);

        var result = await _controller.GetAllOrders();

        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(orders);
    }

    [Fact]
    public async Task GetOrderById_ShouldReturnOk_WhenOrderExists()
    {
        var order = new OrderDto { Id = 1, CustomerId = 3 };

        _orderServiceMock.Setup(service => service.GetOrderByIdAsync(1)).ReturnsAsync(order);

        var result = await _controller.GetOrderById(1);

        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(order);
    }

    [Fact]
    public async Task GetOrderById_ShouldReturnNotFound_WhenOrderDoesNotExist()
    {
        _orderServiceMock.Setup(service => service.GetOrderByIdAsync(99))
            .ThrowsAsync(new KeyNotFoundException("Order with ID 99 not found."));

        var result = await _controller.GetOrderById(99);

        var notFoundResult = result.Result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult.StatusCode.Should().Be(404);
        notFoundResult.Value.Should().Be("Order with ID 99 not found.");
    }

    [Fact]
    public async Task CreateOrder_ShouldReturnCreated_WhenOrderIsValid()
    {
        var orderRequestDto = new OrderRequestDto { CustomerId = 1, OrderDate = DateTime.UtcNow };
        var createdOrder = new OrderDto { Id = 5, CustomerId = 1 };

        _orderServiceMock.Setup(service => service.CreateOrderAsync(orderRequestDto)).ReturnsAsync(createdOrder);

        var result = await _controller.CreateOrder(orderRequestDto);

        var createdResult = result.Result as CreatedAtActionResult;
        createdResult.Should().NotBeNull();
        createdResult.StatusCode.Should().Be(201);
        createdResult.Value.Should().BeEquivalentTo(createdOrder);
    }

    [Fact]
    public async Task CreateOrder_ShouldReturnBadRequest_WhenCreationFails()
    {
        var orderRequestDto = new OrderRequestDto { CustomerId = 1, OrderDate = DateTime.UtcNow };

        _orderServiceMock.Setup(service => service.CreateOrderAsync(orderRequestDto))
            .ThrowsAsync(new InvalidOperationException("Invalid order data."));

        var result = await _controller.CreateOrder(orderRequestDto);

        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
        badRequestResult.Value.Should().Be("Invalid order data.");
    }

    [Fact]
    public async Task UpdateOrder_ShouldReturnOk_WhenOrderIsUpdated()
    {
        var orderRequestDto = new OrderRequestDto { CustomerId = 1, OrderDate = DateTime.UtcNow };
        var updatedOrder = new OrderDto { Id = 5, CustomerId = 1 };

        _orderServiceMock.Setup(service => service.UpdateOrderAsync(5, orderRequestDto)).ReturnsAsync(updatedOrder);

        var result = await _controller.UpdateOrder(5, orderRequestDto);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(updatedOrder);
    }

    [Fact]
    public async Task UpdateOrder_ShouldReturnNotFound_WhenOrderDoesNotExist()
    {
        var orderRequestDto = new OrderRequestDto { CustomerId = 1, OrderDate = DateTime.UtcNow };

        _orderServiceMock.Setup(service => service.UpdateOrderAsync(99, orderRequestDto))
            .ThrowsAsync(new KeyNotFoundException("Order with ID 99 not found."));

        var result = await _controller.UpdateOrder(99, orderRequestDto);

        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult.StatusCode.Should().Be(404);
        notFoundResult.Value.Should().Be("Order with ID 99 not found.");
    }

    [Fact]
    public async Task DeleteOrder_ShouldReturnOk_WhenOrderIsDeleted()
    {
        _orderServiceMock.Setup(service => service.DeleteOrderAsync(1)).Returns(Task.CompletedTask);

        var result = await _controller.DeleteOrder(1);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(new { message = "Order successfully deleted" });
    }

    [Fact]
    public async Task DeleteOrder_ShouldReturnNotFound_WhenOrderDoesNotExist()
    {
        _orderServiceMock.Setup(service => service.DeleteOrderAsync(99))
            .ThrowsAsync(new KeyNotFoundException("Order with ID 99 not found."));

        var result = await _controller.DeleteOrder(99);

        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult.StatusCode.Should().Be(404);
        notFoundResult.Value.Should().Be("Order with ID 99 not found.");
    }
}