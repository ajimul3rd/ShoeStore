using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ShoeStore.Shared.Dto
{
    public class ShoppingCartDto
    {
        public int? CartId { get; set; }

        [Required]
        [ForeignKey("Users")]
        public int UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [JsonIgnore]
        public UsersDto? Users { get; set; }
        public List<CartItemDto>? CartItemDto { get; set; }
    }
}
