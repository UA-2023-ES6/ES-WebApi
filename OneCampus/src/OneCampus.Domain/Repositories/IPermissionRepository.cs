using OneCampus.Domain.Entities.Permissions;

namespace OneCampus.Domain.Repositories
{
    public interface IPermissionRepository
    {
        Task<UserPermissions> AllowPermissionAsync(Guid userID, int groupID, PermissionType permission);

        Task<UserPermissions> DenyPermissionAsync(Guid userID, int groupID, PermissionType permission);

        Task<UserPermissions?> GetPermissions(Guid userID, int groupID);

        Task<UserPermissions> DeleteAsync(Guid userID, int groupID);
    }
}
