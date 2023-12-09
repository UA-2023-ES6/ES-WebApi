namespace OneCampus.Api.Models.Requests.Permissions;

public class UserPermissionsRequest
{
    public Guid UserId { get; set; }

    public int GroupId { get; set; }
}
