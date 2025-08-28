using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ShoeStore.Shared.Enums;

namespace ShoeStore.Model.Entity
{
    public class ShippingAddress
    {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int AddressId { get; set; }

            [Required]
            [ForeignKey("Users")]
            public int UserId { get; set; }

            [Required]
            [MaxLength(200)]
            public string? AddressLine1 { get; set; }

            [MaxLength(200)]
            public string? AddressLine2 { get; set; }

            [Required]
            [MaxLength(100)]
            public string? City { get; set; }

            [Required]
            [MaxLength(100)]
            public string? State { get; set; }

            [Required]
            [MaxLength(10)]
            public string? PostalCode { get; set; }

            [Required]
            [MaxLength(100)]
            public string? Country { get; set; } = "India";

            [Required]
            [MaxLength(100)]
            public string? RecipientName { get; set; }

            [Phone]
            [MaxLength(15)]
            public string? RecipientPhone { get; set; }

            public bool IsDefault { get; set; } = false;

            public AddressType AddressType { get; set; } = AddressType.Home;

            public bool IsActive { get; set; } = true;

            // Navigation property
            [JsonIgnore]
            public  Users Users { get; set; }
        }
    }

