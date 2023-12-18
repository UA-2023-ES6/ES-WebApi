using Microsoft.EntityFrameworkCore;
using OneCampus.Domain;
using OneCampus.Domain.Exceptions;
using OneCampus.Domain.Services;
using OneCampus.Infrastructure.Data;
using Database = OneCampus.Infrastructure.Data.Entities;

namespace OneCampus.Api;

public class DebugDataService
{
    public static readonly Guid RootUser = Guid.Parse("00000000-0000-0000-0000-000000000001");

    private const int DefaultInstitutionId = 1;

    private readonly IDbContextFactory<OneCampusDbContext> _dbContextFactory;
    private readonly Lazy<IUsersService> _lazyUsersService;

    private IUsersService _usersService => _lazyUsersService.Value;

    private static int DefaultInstitutionGroupId = 0;

    public DebugDataService(
        IDbContextFactory<OneCampusDbContext> dbContextFactory,
        Lazy<IUsersService> lazyUsersService)
    {
        _dbContextFactory = dbContextFactory;
        _lazyUsersService = lazyUsersService;
    }

    internal async Task CreateDefaultDataAsync()
    {
        await AddPermissions();

        await AddUserAsync(RootUser, "User 1", "user1@OneCampus.pt");
        var institution = await AddInstitutionAsync(RootUser, "Default Institution", DefaultInstitutionId);
        DefaultInstitutionGroupId = institution.GroupId;
        await AddAllPermissionsToUser(RootUser, DefaultInstitutionGroupId);

        var user = await AddUserAsync(Guid.Parse("00000000-0000-0000-0000-000000000002"), "User 2", "user2@OneCampus.pt");
        await AddUserToGroupAsync(DefaultInstitutionGroupId, user.Id);
        await AddAllPermissionsToUser(user.Id, DefaultInstitutionGroupId);

        user = await AddUserAsync(Guid.Parse("00000000-0000-0000-0000-000000000003"), "User 3", "user3@OneCampus.pt");
        await AddUserToGroupAsync(DefaultInstitutionGroupId, user.Id);
        await AddAllPermissionsToUser(user.Id, DefaultInstitutionGroupId);

        user = await AddUserAsync(Guid.Parse("00000000-0000-0000-0000-000000000004"), "User 4", "user4@OneCampus.pt");
        await AddUserToGroupAsync(DefaultInstitutionGroupId, user.Id);
        await AddAllPermissionsToUser(user.Id, DefaultInstitutionGroupId);
    }

    internal async Task AddUserToDefaultInstitution(Guid userId)
    {
        await AddUserToGroupAsync(DefaultInstitutionGroupId, userId);
        await AddAllPermissionsToUser(userId, DefaultInstitutionGroupId);
    }

    internal async Task<Database.Institution> AddInstitutionAsync(Guid userId, string name, int? id = null)
    {
        using (var context = await _dbContextFactory.CreateDbContextAsync())
        {
            Database.Institution? institution = null;
            if (id.HasValue)
            {
                institution = await context.Institutions.FindAsync(id.Value);
                if (institution is not null)
                {
                    return institution;
                }
            }

            var user = await context.Users.FindAsync(userId);

            var defaultGroup = new Database.Group
            {
                Name = name,
                CreateDate = DateTime.UtcNow
            };

            var result = await context.Institutions
                .AddAsync(new Database.Institution
                {
                    Id = id ?? 0,
                    Name = name,
                    CreateDate = DateTime.UtcNow,
                    Group = defaultGroup
                });

            await context.SaveChangesAsync();

            var userGroup = new Database.UserGroup
            {
                Group = result.Entity.Group,
                User = user!
            };

            await context.UserGroups.AddAsync(userGroup);

            await context.SaveChangesAsync();

            return result.Entity;
        }
    }

    internal async Task<Domain.Entities.Users.User> AddUserAsync(Guid id, string username, string email)
    {
        var user = await _usersService.FindAsync(id);
        if (user is not null)
        {
            return user;
        }

        return await _usersService.CreateAsync(id, username, email);
    }

    internal async Task AddUserToGroupAsync(int groupId, Guid userId)
    {
        using (var context = await _dbContextFactory.CreateDbContextAsync())
        {
            var group = await context.Groups
                .Include(item => item.UserGroups)
                .FirstOrDefaultAsync(item => item.DeleteDate == null && item.Id == groupId);
            if (group == null)
            {
                throw new NotFoundException($"goup with id {groupId} not found");
            }

            if (group.UserGroups.Any(item => item.UserId == userId))
            {
                return;
            }

            var newUser = await context.Users.FirstOrDefaultAsync(item => item.DeleteDate == null && item.Id == userId);
            if (newUser is not null)
            {
                var userGroup = new Database.UserGroup
                {
                    Group = group,
                    User = newUser
                };

                group.UserGroups.Add(userGroup);

                await context.SaveChangesAsync();
            }
        }
    }

    internal async Task AddPermissions()
    {
        using (var context = await _dbContextFactory.CreateDbContextAsync())
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

    internal async Task AddAllPermissionsToUser(Guid userId, int groupId)
    {
        using (var context = await _dbContextFactory.CreateDbContextAsync())
        {
            var user = await context.Users.FindAsync(userId);
            var permissions = await context.Permissions.ToListAsync();
            var userGroup = await context.UserGroups.FirstOrDefaultAsync(item =>
                item.UserId == userId &&
                item.GroupId == groupId);
            if (userGroup is null)
            {
                throw new ArgumentNullException(nameof(userGroup));
            }

            foreach (var permission in permissions)
            {
                var current = await context.UserGroupPermissions.FirstOrDefaultAsync(item =>
                    item.UserGroupId == userGroup.Id &&
                    item.PermissionId == permission.Id);
                if (current is null)
                {
                    await context.UserGroupPermissions.AddAsync(new Database.UserGroupPermissions
                    {
                        Permission = permission,
                        UserGroup = userGroup
                    });
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
