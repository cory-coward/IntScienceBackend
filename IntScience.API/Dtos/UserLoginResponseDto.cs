namespace IntScience.API.Dtos;

public class UserLoginResponseDto
{
    public bool IsAuthenticationSuccessful { get; set; }
    public string ErrorMessage { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public IList<string> Roles { get; set; }
    public string Token { get; set; }
}
