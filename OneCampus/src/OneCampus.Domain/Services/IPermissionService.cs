using OneCampus.Domain.Entities.Permissions;

namespace OneCampus.Domain.Services;

public interface IPermissionService
{
    Task<UserPermissions> AllowPermissionsAsync(Guid userId, int groupId, IList<PermissionType> permissions);

    Task<UserPermissions> DenyPermissionsAsync(Guid userId, int groupId, IList<PermissionType> permissions);

    Task<UserPermissions> GetPermissionsAsync(int groupId);

    Task<UserPermissions> GetPermissionsAsync(Guid userId, int groupId);
}
