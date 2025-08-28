using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ShoeStore.Model.Entity
{
    public class Users
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        public string? UserName { get; set; }

        public string? UserEmail { get; set; }

        public string? UserContact { get; set; }

        public string? UserPassword { get; set; }

        public bool IsActive { get; set; } = true;

        public string? RefreshToken { get; set; } // "?" makes it nullable

        public DateTime? RefreshTokenExpiry { get; set; }

        public List<ShippingAddress>? Address { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
