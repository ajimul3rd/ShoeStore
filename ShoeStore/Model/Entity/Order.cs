using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ShoeStore.Shared.Enums;

namespace ShoeStore.Model.Entity
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }

        [Required]
        public string OrderNumber { get; set; } = GenerateOrderNumber();

        [Required]
        [ForeignKey("Users")]
        public int UserId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal OrderTotal { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxAmount { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ShippingCharge { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal FinalAmount { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
        public PaymentMethod PaymentMethod { get; set; }

        [MaxLength(50)]
        public string? TransactionId { get; set; }

        [Required]
        [ForeignKey("ShippingAddress")]
        public int ShippingAddressId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public DateTime? ExpectedDeliveryDate { get; set; }
        public DateTime? DeliveredDate { get; set; }

        // Navigation properties
        [JsonIgnore]
        public Users? Users { get; set; }
        public ShippingAddress? ShippingAddress { get; set; }
        public List<OrderItem>? OrderItems { get; set; }
        public List<OrderStatusHistory>? StatusHistory { get; set; }

        private static string GenerateOrderNumber()
        {
            return "ORD" + DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
        }
    }
}
