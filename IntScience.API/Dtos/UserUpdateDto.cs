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

    [Required]
    [MinLength(1, ErrorMessage = "You must specify at least one role for this user.")]
    public IEnumerable<string> Roles { get; set; }
}
