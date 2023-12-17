using OneCampus.Domain;
using OneCampus.Domain.Exceptions;
using OneCampus.Infrastructure.Data.Entities;

namespace OneCampus.Infrastructure.Tests.Helpers;

public static class PermissionHelper
{
    public static async Task AddPermissions(IDbContextFactory<OneCampusDbContext> dbContextFactory)
    {
        using (var context = await dbContextFactory.CreateDbContextAsync())
        {
            var permissionNames = Enum.GetNames<PermissionType>();
            foreach (var permissionName in permissionNames)
            {
                var value = (int)Enum.Parse<PermissionType>(permissionName)!;

                var permission = await context.Permissions.FindAsync(value);
                if (permission is null)
                {
                    await context.AddAsync(new Database.Permission
                    {
                        Id = value,
                        Name = permissionName,
                        CreateDate = DateTime.UtcNow
                    });
                }
            }

            await context.SaveChangesAsync();
        }
    }

    public static async Task AddPermissionsAsync(
        IDbContextFactory<OneCampusDbContext> dbContextFactory,
        int groupId,
        Guid userId,        
        params PermissionType[] permissions)
    {
        using (var context = await dbContextFactory.CreateDbContextAsync())
        {
            var usergroup = await context.UserGroups.FirstAsync(item => item.UserId == userId && item.GroupId == groupId);
            foreach (var permissionType in permissions)
            {
                var permission = await context.Permissions.FirstAsync(item => item.Id == (int)permissionType);
                if (permission == null)
                {
                    throw new NotFoundException("permission not found");
                }

                var userGroupPermission = new UserGroupPermissions
                {
                    UserGroup = usergroup,
                    Permission = permission
                };

                var result = await context.UserGroupPermissions.AddAsync(userGroupPermission);
            }

            await context.SaveChangesAsync();
        }
    }
}