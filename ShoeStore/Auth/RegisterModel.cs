using ShoeStore.Model.Entity;
using ShoeStore.Shared.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ShoeStore.Auth;

public class RegisterModel
{
    public int? UserId { get; set; } = 0;

    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, ErrorMessage = "Username must be under 50 characters")]
    public string? UserName { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string? UserEmail { get; set; }

    //[Required(ErrorMessage = "Contact number is required")]
    //[Phone(ErrorMessage = "Invalid phone number")]
    public string? UserContact { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    public string? UserPassword { get; set; }

    [Required(ErrorMessage = "Confirm Password is required")]
    [DataType(DataType.Password)]
    [Compare("UserPassword", ErrorMessage = "Confirm Password must match with Password")]
    public string? ConfirmPassword { get; set; }

    [Range(typeof(bool), "true", "true", ErrorMessage = "You must agree to terms")]
    public bool AgreeToTerms { get; set; }


    //[Required(ErrorMessage = "User RoleMust be define")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public UserRole? Role { get; set; }
    public bool IsActive { get; set; } = true;

    public List<ShippingAddress>? Address { get; set; }
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

}
