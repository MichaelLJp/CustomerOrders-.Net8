using System.Text.Json.Serialization;

namespace CustomerOrders.Application.Dtos
{
    public class OrderRequestDto
    {
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
