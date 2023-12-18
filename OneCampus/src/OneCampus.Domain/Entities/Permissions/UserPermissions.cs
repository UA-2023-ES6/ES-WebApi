namespace OneCampus.Domain.Entities.Permissions;

public sealed record UserPermissions
{
    public IList<PermissionType> Permissions { get; }

    public Guid UserId { get; }

    public int GroupId { get; }

    public UserPermissions(Guid userId, int groupId) : this(userId, groupId, new List<PermissionType>(0))
    { }

    public UserPermissions(Guid userId, int groupId, IList<PermissionType> permissions)
    {
        UserId = userId.Throw().IfDefault().Value;
        GroupId = groupId.Throw().IfNegativeOrZero().Value;
        Permissions = permissions.ThrowIfNull().Value;
    }
}
