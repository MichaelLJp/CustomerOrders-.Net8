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
public class OrderServiceTests
{
    private readonly Mock<IRepository<Order>> _orderRepositoryMock;
    private readonly Mock<IRepository<Customer>> _customerRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        _orderRepositoryMock = new Mock<IRepository<Order>>();
        _customerRepositoryMock = new Mock<IRepository<Customer>>();
        _mapperMock = new Mock<IMapper>();
        _orderService = new OrderService(_orderRepositoryMock.Object, _customerRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetAllOrdersAsync_ShouldReturnOrders_WhenOrdersExist()
    {
        var orders = new List<Order>
        {
            new Order { Id = 1, CustomerId = 3 },
            new Order { Id = 2, CustomerId = 4 }
        };

        _orderRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(orders);
        _mapperMock.Setup(map => map.Map<IEnumerable<OrderDto>>(orders)).Returns(new List<OrderDto>
        {
            new OrderDto { Id = 1, CustomerId = 3 },
            new OrderDto { Id = 2, CustomerId = 4 }
        });

        var result = await _orderService.GetAllOrdersAsync();

        result.Should().NotBeNullOrEmpty();
        result.Should().HaveCount(2);
        result.First().Id.Should().Be(1);
        result.Last().Id.Should().Be(2);
    }


    [Fact]
    public async Task GetOrderByIdAsync_ShouldThrowException_WhenOrderDoesNotExist()
    {
        _orderRepositoryMock.Setup(repo => repo.GetByIdAsync(99)).ReturnsAsync((Order?)null);

        var act = async () => await _orderService.GetOrderByIdAsync(99);
        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Order with ID 99 not found.");
    }
    [Fact]
    public async Task GetOrderByIdAsync_ShouldReturnOrder_WhenExists()
    {
        var order = new Order { Id = 1, CustomerId = 5 };
        _orderRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(order);
        _mapperMock.Setup(map => map.Map<OrderDto>(order)).Returns(new OrderDto { Id = 1, CustomerId = 5 });

        var result = await _orderService.GetOrderByIdAsync(1);

        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.CustomerId.Should().Be(5);
    }
    [Fact]
    public async Task CreateOrderAsync_ShouldCreateOrder_WhenCustomerExists()
    {
        var orderRequestDto = new OrderRequestDto { CustomerId = 1, OrderDate = DateTime.UtcNow };
        var order = new Order { Id = 5, CustomerId = 1, OrderDate = orderRequestDto.OrderDate };

        _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(new Customer { Id = 1 });
        _orderRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);
        _mapperMock.Setup(map => map.Map<Order>(orderRequestDto)).Returns(order);
        _mapperMock.Setup(map => map.Map<OrderDto>(order)).Returns(new OrderDto { Id = 5, CustomerId = 1 });

        var result = await _orderService.CreateOrderAsync(orderRequestDto);

        result.Should().NotBeNull();
        result.Id.Should().Be(5);
        result.CustomerId.Should().Be(1);
    }
    [Fact]
    public async Task CreateOrderAsync_ShouldThrowException_WhenCustomerDoesNotExist()
    {
        var orderRequestDto = new OrderRequestDto { CustomerId = 99, OrderDate = DateTime.UtcNow };

        _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(99)).ReturnsAsync((Customer?)null);

        var act = async () => await _orderService.CreateOrderAsync(orderRequestDto);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("The specified customer does not exist, please verify the entered CustomerId.");
    }
    
    [Fact]
    public async Task UpdateOrderAsync_ShouldUpdateOrder_WhenOrderExists()
    {
        var orderRequestDto = new OrderRequestDto { CustomerId = 3, OrderDate = DateTime.UtcNow };
        var existingOrder = new Order { Id = 1, CustomerId = 5, OrderDate = DateTime.UtcNow };

        _orderRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingOrder);
        _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(orderRequestDto.CustomerId)).ReturnsAsync(new Customer { Id = 3 });

        _mapperMock.Setup(map => map.Map(orderRequestDto, existingOrder))
            .Callback<OrderRequestDto, Order>((dto, order) => order.CustomerId = dto.CustomerId); 

        await _orderService.UpdateOrderAsync(1, orderRequestDto);

        existingOrder.CustomerId.Should().Be(3);
    }
    [Fact]
    public async Task UpdateOrderAsync_ShouldThrowException_WhenOrderDoesNotExist()
    {
        var orderRequestDto = new OrderRequestDto { CustomerId = 3, OrderDate = DateTime.UtcNow };

        _orderRepositoryMock.Setup(repo => repo.GetByIdAsync(99)).ReturnsAsync((Order?)null);

        var act = async () => await _orderService.UpdateOrderAsync(99, orderRequestDto);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Order with ID 99 not found.");
    }
    [Fact]
    public async Task UpdateOrderAsync_ShouldThrowException_WhenCustomerDoesNotExist()
    {
        var orderRequestDto = new OrderRequestDto { CustomerId = 99, OrderDate = DateTime.UtcNow };
        var existingOrder = new Order { Id = 1, CustomerId = 5, OrderDate = DateTime.UtcNow };

        _orderRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingOrder);
        _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(orderRequestDto.CustomerId)).ReturnsAsync((Customer?)null);

        var act = async () => await _orderService.UpdateOrderAsync(1, orderRequestDto);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Customer with ID {orderRequestDto.CustomerId} does not exist. Cannot assign a non-existent customer.");
    }

    [Fact]
    public async Task DeleteOrderAsync_ShouldDeleteOrder_WhenOrderExists()
    {
        var existingOrder = new Order { Id = 1, CustomerId = 5 };

        _orderRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingOrder);
        _orderRepositoryMock.Setup(repo => repo.DeleteAsync(1)).ReturnsAsync(true); 

        await _orderService.DeleteOrderAsync(1);

        _orderRepositoryMock.Verify(repo => repo.DeleteAsync(1), Times.Once);
    }
    [Fact]
    public async Task DeleteOrderAsync_ShouldThrowException_WhenOrderDoesNotExist()
    {
        _orderRepositoryMock.Setup(repo => repo.GetByIdAsync(99)).ReturnsAsync((Order?)null);

        var act = async () => await _orderService.DeleteOrderAsync(99);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Order with ID 99 not found.");
    }
  
}
