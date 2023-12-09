using Microsoft.EntityFrameworkCore;
using OneCampus.Domain;
using OneCampus.Domain.Entities.Permissions;
using OneCampus.Domain.Repositories;
using OneCampus.Infrastructure.Data;
using OneCampus.Infrastructure.Data.Entities;
using OneCampus.Infrastructure.Extensions;
using System.Collections.Immutable;

namespace OneCampus.Infrastructure.Repositories;

public class PermissionsRepository : IPermissionRepository
{
    private readonly IDbContextFactory<OneCampusDbContext> _oneCampusDbContextFactory;

    public PermissionsRepository(IDbContextFactory<OneCampusDbContext> oneCampusDbContextFactory)
    {
        _oneCampusDbContextFactory = oneCampusDbContextFactory.ThrowIfNull().Value;
    }

    public async Task<UserPermissions?> AllowPermissionsAsync(Guid userId, int groupId, IList<PermissionType> permissions)
    {
        using (var context = _oneCampusDbContextFactory.CreateDbContext())
        {
            var usergroup = await context.UserGroups.FirstOrDefaultAsync(item => item.UserId == userId && item.GroupId == groupId);
            if (usergroup == null)
            {
                return null;
            }

            var currentPermissions  = await GetPermissionsAsync(context, userId, groupId);
            if (currentPermissions == null)
            {
                return null;
            }

            foreach (var permissionType in permissions)
            {
                if (currentPermissions.Permissions.Contains(permissionType))
                {
                    continue;
                }

                var permission = await context.Permissions.FindAsync((int)permissionType);
                if (permission == null)
                {
                    return null;
                }

                var userGroupPermission = new UserGroupPermissions
                {
                    UserGroup = usergroup,
                    Permission = permission
                };

                var result = await context.UserGroupPermissions.AddAsync(userGroupPermission);
            }

            await context.SaveChangesAsync();

            return await GetPermissionsAsync(context, userId, groupId);
        }
    }

    public async Task<UserPermissions?> DenyPermissionsAsync(Guid userId, int groupId, IList<PermissionType> permissions)
    {
        var permissionsIds = permissions.Select(item => (int)item);

        using (var context = _oneCampusDbContextFactory.CreateDbContext())
        {
            var permissionsToRemove = await context.UserGroupPermissions
                .Where(item =>
                    item.UserGroup.GroupId == groupId &&
                    item.UserGroup.UserId == userId &&
                    permissionsIds.Contains(item.PermissionId))
                .ToListAsync();
            if (permissionsToRemove.Any())
            {
                context.UserGroupPermissions.RemoveRange(permissionsToRemove);

                await context.SaveChangesAsync();
            }

            return await GetPermissionsAsync(context, userId, groupId);
        }
    }

    public async Task<UserPermissions?> GetPermissionsAsync(Guid userId, int groupId)
    {
        using (var context = _oneCampusDbContextFactory.CreateDbContext())
        {
            return await GetPermissionsAsync(context, userId, groupId);
        }
    }

    public async Task<bool> UserHasPermissionAsync(Guid userId, int groupId, PermissionType permissionType)
    {
        using (var context = _oneCampusDbContextFactory.CreateDbContext())
        {
            return await context.UserGroupPermissions
                .AnyAsync(item =>
                    item.UserGroup.GroupId == groupId &&
                    item.UserGroup.UserId == userId &&
                    item.PermissionId == (int)permissionType);
        }
    }

    private async Task<UserPermissions?> GetPermissionsAsync(OneCampusDbContext context, Guid userId, int groupId)
    {
        var permissions = await context.UserGroupPermissions
            .Where(item => item.UserGroup.GroupId == groupId && item.UserGroup.UserId == userId)
            .ToListAsync();

        return permissions.ToUserPermissions(userId, groupId);
    }
}
