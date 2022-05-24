using System.ComponentModel.DataAnnotations;

namespace IntScience.API.Dtos;

public class UserUpdateDto
{
    [Required(ErrorMessage = "User ID is required.")]
    public string Id { get; set; }

    [Required(ErrorMessage = "First name is required.")]
    [MaxLength(50)]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last name is required.")]
    [MaxLength(50)]
    public string LastName { get; set; }

    [Required(ErrorMessage = "At least one user role is required.")]
    public IList<string> Roles { get; set; }
}
