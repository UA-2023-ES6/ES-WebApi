using OneCampus.Domain;
using OneCampus.Domain.Entities.Permissions;
using Database = OneCampus.Infrastructure.Data.Entities;

namespace OneCampus.Infrastructure.Extensions;

internal static class PermissionExtentions
{
    internal static UserPermissions? ToUserPermissions(
        this IEnumerable<Database.UserGroupPermissions?> userGroupPermissions,
        Guid userId,
        int groupId)
    {
        if (userGroupPermissions is null)
        {
            return null;
        }

        var permissions = userGroupPermissions
            .Where(item => item is not null)
            .Select(item => (PermissionType)item!.PermissionId)
            .ToList();

        return new UserPermissions(userId, groupId, permissions);
    }
}
