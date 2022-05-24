namespace IntScience.API.Dtos;

public class UserProfileResponseDto
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public ICollection<string> Roles { get; set; }
}
