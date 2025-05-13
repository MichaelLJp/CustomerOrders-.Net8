using Microsoft.EntityFrameworkCore;
using CustomerOrders.Domain.Interfaces;
using CustomerOrders.Infrastructure.Data;
using CustomerOrders.Application.Services;
using CustomerOrders.Application.Mappings;
using CustomerOrders.Infrastructure.Repositories;
using Microsoft.AspNetCore.Diagnostics;
using AutoMapper;
using CustomerOrders.Application.Interfaces;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CustomerOrdersConnection")));
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CustomerOrders API",
        Version = "1.0",
        Description = "API for managing customer orders with ASP.NET Core and Clean Architecture.",
        Contact = new OpenApiContact
        {
            Name = "Michael Leonardo Uyaban",
            Email = "colombiamichael4@gmail.com",
            Url = new Uri("https://github.com/MichaelLJp")
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync($"Internal Server Error: {exception?.Message}");
    });
});
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
