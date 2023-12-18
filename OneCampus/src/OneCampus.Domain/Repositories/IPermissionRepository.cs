using OneCampus.Domain.Entities.Permissions;

namespace OneCampus.Domain.Repositories;

public interface IPermissionRepository
{
    Task<UserPermissions?> AllowPermissionsAsync(Guid userId, int groupId, IList<PermissionType> permissions);

    Task<UserPermissions?> DenyPermissionsAsync(Guid userId, int groupId, IList<PermissionType> permissions);

    Task<UserPermissions?> GetPermissionsAsync(Guid userId, int groupId);

    Task<bool> UserHasPermissionAsync(Guid userId, int groupId, PermissionType permissionType);
}
