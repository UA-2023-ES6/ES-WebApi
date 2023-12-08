using OneCampus.Domain.Entities.Permissions;

namespace OneCampus.Domain.Services
{
    public interface IPermissionService
    {
        Task<UserPermissions> AllowPermissionAsync(Guid userID, int groupID, Permissions permission);

        Task<UserPermissions> DenyPermissionAsync(Guid userID, int groupID, Permissions permission);

        Task<UserPermissions> GetPermissions(Guid userID, int groupID);

        Task<UserPermissions> DeleteAsync(Guid userID, int groupID);
    }
}
