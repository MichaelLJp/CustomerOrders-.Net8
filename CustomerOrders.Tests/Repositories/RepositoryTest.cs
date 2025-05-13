using Xunit;
using Microsoft.EntityFrameworkCore;
using CustomerOrders.Domain.Entities;
using CustomerOrders.Domain.Interfaces;
using CustomerOrders.Infrastructure.Data;
using CustomerOrders.Infrastructure.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;

namespace CustomerOrders.Tests.Repositories;

public class RepositoryTests
{
    private readonly AppDbContext _context;
    private readonly IRepository<Order> _orderRepository;

    public RepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new AppDbContext(options);
        _orderRepository = new Repository<Order>(_context);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnOrders_WhenOrdersExist()
    {
        _context.Orders.AddRange(new Order { Id = 1, CustomerId = 3 }, new Order { Id = 2, CustomerId = 4 });
        await _context.SaveChangesAsync();

        var result = await _orderRepository.GetAllAsync();

        result.Should().NotBeNullOrEmpty();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnOrder_WhenOrderExists()
    {
        var order = new Order { CustomerId = 3 };  
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        var result = await _orderRepository.GetByIdAsync(order.Id);  

        result.Should().NotBeNull();
        result!.Id.Should().Be(order.Id);
        result.CustomerId.Should().Be(3);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenOrderDoesNotExist()
    {
        var result = await _orderRepository.GetByIdAsync(99);

        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_ShouldAddOrder_WhenCustomerExists()
    {
        if (!await _context.Customers.AnyAsync(c => c.Id == 3)) 
        {
            _context.Customers.Add(new Customer { Id = 3, Name = "Test Customer", Email = "test@example.com" });
            await _context.SaveChangesAsync();
        }

        var order = new Order { CustomerId = 3 };
        await _orderRepository.CreateAsync(order);

        var savedOrder = await _context.Orders.FirstOrDefaultAsync(o => o.CustomerId == 3);
        savedOrder.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowException_WhenCustomerDoesNotExist()
    {
        var order = new Order { CustomerId = 99 };

        var act = async () => await _orderRepository.CreateAsync(order);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("El cliente especificado no existe.");
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyOrder_WhenCustomerExists()
    {
        _context.Customers.Add(new Customer { Id = 3, Name = "Test Customer", Email = "test@example.com" });
        await _context.SaveChangesAsync();

        var order = new Order { CustomerId = 5 };  
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        var existingOrder = await _context.Orders.FirstAsync();  
        existingOrder.CustomerId = 3;  

        await _orderRepository.UpdateAsync(existingOrder);

        var updatedOrder = await _context.Orders.FindAsync(existingOrder.Id);
        updatedOrder.Should().NotBeNull();
        updatedOrder!.CustomerId.Should().Be(3);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowException_WhenCustomerDoesNotExist()
    {
        if (!await _context.Customers.AnyAsync(c => c.Id == 3))
        {
            _context.Customers.Add(new Customer { Id = 3, Name = "Valid Customer", Email = "customer@example.com" });
            await _context.SaveChangesAsync();
        }
        if (!await _context.Orders.AnyAsync(o => o.Id == 1))
        {
            _context.Orders.Add(new Order { Id = 1, CustomerId = 3 });
            await _context.SaveChangesAsync();
        }
        var order = await _context.Orders.FindAsync(1);
        _context.Entry(order!).State = EntityState.Detached;
        order!.CustomerId = 99;

        var act = async () => await _orderRepository.UpdateAsync(order);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("No se puede asignar un cliente inexistente.");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveOrder_WhenOrderExists()
    {
        var order = new Order { CustomerId = 5 };  
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        var deleted = await _orderRepository.DeleteAsync(order.Id);  

        deleted.Should().BeTrue();
        var deletedOrder = await _context.Orders.FindAsync(order.Id);
        deletedOrder.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenOrderDoesNotExist()
    {
        var deleted = await _orderRepository.DeleteAsync(99);

        deleted.Should().BeFalse();
    }
}