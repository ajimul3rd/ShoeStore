
namespace ShoeStore.Shared.Dto
{
    public class UsersDto
    {
        public int? UserId { get; set; }

        public string? UserName { get; set; }

        public string? UserEmail { get; set; }

        public string? UserContact { get; set; }

        public string? UserPassword { get; set; }

        public bool IsActive { get; set; } = true;

        public string? RefreshToken { get; set; } // "?" makes it nullable

        public DateTime? RefreshTokenExpiry { get; set; }

        public List<ShippingAddressDto>? ShippingAddressDto { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
