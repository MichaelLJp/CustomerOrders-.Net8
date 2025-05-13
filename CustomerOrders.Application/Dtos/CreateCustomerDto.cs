using System.Text.Json.Serialization;

namespace CustomerOrders.Application.Dtos
{
    public class CustomerRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
