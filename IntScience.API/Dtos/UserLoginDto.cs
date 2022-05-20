using System.ComponentModel.DataAnnotations;

namespace IntScience.API.Dtos;

public class UserLoginDto
{
    [Required(ErrorMessage = "Email address is required")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }
}
