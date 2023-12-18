using Microsoft.EntityFrameworkCore;

namespace OneCampus.Infrastructure.Data.Entities;

[PrimaryKey(nameof(PermissionId), nameof(UserGroupId))]
public class UserGroupPermissions
{
    public int PermissionId { get; set; }
    public int UserGroupId { get; set; }

    public virtual Permission Permission { get; set; } = null!;
    public virtual UserGroup UserGroup { get; set; } = null!;
}
