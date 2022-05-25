namespace IntScience.API.Dtos;

public class UserListResponseDto
{
    public int UserCount { get; set; }
    public int PageCount { get; set; }
    public IEnumerable<UserProfileResponseDto> Users { get; set; }
}
