namespace OneCampus.Api.Models;

public class UserInfo
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
}