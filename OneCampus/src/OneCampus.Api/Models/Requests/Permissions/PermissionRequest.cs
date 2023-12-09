using OneCampus.Domain;

namespace OneCampus.Api.Models.Requests.Permissions;

public class PermissionRequest
{
    public List<PermissionType> Permissions { get; set; } = new();
}
