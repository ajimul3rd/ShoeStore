using System.ComponentModel.DataAnnotations;

namespace ShoeStore.Auth;

public class LoginModel
{
    [Required(ErrorMessage = "User name is required")]
    public  string? UserName { get; set; }
    [Required(ErrorMessage = "Password is required")]
    public  string? UserPassword { get; set; }
}
