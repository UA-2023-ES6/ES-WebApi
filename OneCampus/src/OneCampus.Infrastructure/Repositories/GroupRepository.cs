﻿using Microsoft.EntityFrameworkCore;
using OneCampus.Domain.Entities.Groups;
using OneCampus.Domain.Repositories;
using OneCampus.Infrastructure.Data;
using Database = OneCampus.Infrastructure.Data.Entities;

namespace OneCampus.Infrastructure.Repositories;

public class GroupRepository : IGroupRepository
{
    private readonly IDbContextFactory<OneCampusDbContext> _oneCampusDbContextFactory;

    public GroupRepository(IDbContextFactory<OneCampusDbContext> oneCampusDbContextFactory)
    {
        _oneCampusDbContextFactory = oneCampusDbContextFactory.ThrowIfNull().Value;
    }

    public async Task<Group> CreateAsync(string name, int parentGroupId)
    {
        using (var context = await _oneCampusDbContextFactory.CreateDbContextAsync())
        {
            var group = new Database.Group
            {
                Name = name,
                ParentId = parentGroupId,
                CreateDate = DateTime.UtcNow
            };

            var result = await context.Groups.AddAsync(group);

            await context.SaveChangesAsync();

            return result.Entity.ToGroup()!;
        }
    }

    public async Task<Group?> UpdateAsync(int id, string name)
    {
        using (var context = await _oneCampusDbContextFactory.CreateDbContextAsync())
        {
            var group = await context.Groups.FirstOrDefaultAsync(item => item.DeleteDate == null && item.Id == id);
            if (group is null)
            {
                return null;
            }

            group.Name = name;
            group.UpdateDate = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return group.ToGroup();
        }
    }

    public async Task<GroupDetails?> FindAsync(int id)
    {
        using (var context = await _oneCampusDbContextFactory.CreateDbContextAsync())
        {
            var group = await context.Groups
                .AsNoTracking()
                .Include(item => item.UserGroups)
                .ThenInclude(item => item.User)
                .FirstOrDefaultAsync(item => item.DeleteDate == null && item.Id == id);

            return group is null
                ? null
                : group.ToGroupDetais(group.UserGroups.Select(item => item.User.ToUser()!));
        }
    }

    public async Task<Group?> FindByInstitutionIdAsync(int institutionId)
    {
        using (var context = await _oneCampusDbContextFactory.CreateDbContextAsync())
        {
            var group = await context.Groups
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.DeleteDate == null &&
                    item.Institution!.DeleteDate == null &&
                    item.Institution!.Id == institutionId);

            return group.ToGroup();
        }
    }

    public async Task<IEnumerable<Group>> GetSubGroupsAsync(Guid userId, int id)
    {
        using (var context = await _oneCampusDbContextFactory.CreateDbContextAsync())
        {
            var groups = await context.Groups
                .AsNoTracking()
                .Where(item => item.DeleteDate == null &&
                    item.ParentId == id &&
                    item.UserGroups.Any(item => item.UserId == userId))
                .ToListAsync();

            return groups.Select(group => group.ToGroup()!);
        }
    }

    public async Task<Group?> DeleteAsync(int id)
    {
        using (var context = await _oneCampusDbContextFactory.CreateDbContextAsync())
        {
            var group = await context.Groups
                .Include(item => item.Groups)
                .FirstOrDefaultAsync(item => item.DeleteDate == null && item.Id == id);
            if (group is null)
            {
                return null;
            }

            if (group.Groups.Any(item => item.DeleteDate == null))
            {
                throw new NotSupportedException("The group has subgroups");
            }

            group.DeleteDate = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return group.ToGroup()!;
        }
    }

    public async Task<GroupDetails?> AddUserAsync(int groupId, Guid userId)
    {
        using (var context = await _oneCampusDbContextFactory.CreateDbContextAsync())
        {
            var group = await context.Groups
                .Include(item => item.UserGroups)
                .FirstOrDefaultAsync(item => item.DeleteDate == null && item.Id == groupId);
            if (group == null)
            {
                return null;
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

            return group.ToGroupDetais(group.UserGroups.Select(item => item.User.ToUser()!))!;
        }
    }

    public async Task<GroupDetails?> RemoveUserAsync(int groupId, Guid userId)
    {
        using (var context = await _oneCampusDbContextFactory.CreateDbContextAsync())
        {
            var group = await context.Groups
                .Include(item => item.UserGroups)
                .ThenInclude(item => item.User)
                .FirstOrDefaultAsync(item => item.DeleteDate == null && item.Id == groupId);
            if (group == null)
            {
                return null;
            }

            var userGroupToRemove = group.UserGroups.FirstOrDefault(item => item.UserId == userId);
            if (userGroupToRemove is not null)
            {
                context.Remove(userGroupToRemove);

                await context.SaveChangesAsync();
            }

            return group.ToGroupDetais(group.UserGroups.Select(item => item.User.ToUser()!))!;
        }
    }

    public async Task<(IEnumerable<User> Results, int TotalResults)> GetUsersAsync(int id, int take, int skip)
    {
        using (var context = await _oneCampusDbContextFactory.CreateDbContextAsync())
        {
            var query = context.Users.Where(item => item.DeleteDate == null && item.UserGroups.Any(item => item.GroupId == id));

            var totalResults = await query.CountAsync();
            if (totalResults == 0)
            {
                return (Enumerable.Empty<User>(), 0);
            }

            var users = await query
                .OrderBy(item => item.Username)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return (users.Select(item => item.ToUser()!), totalResults);
        }
    }

    public async Task<bool> HasAccessAsync(Guid userId, int groupId)
    {
        using (var context = await _oneCampusDbContextFactory.CreateDbContextAsync())
        {
            return await context.Groups
                .AsNoTracking()
                .Include(item => item.UserGroups)
                .AnyAsync(item => item.DeleteDate == null &&
                    item.Id == groupId &&
                    item.UserGroups.Any(item => item.UserId == userId));
        }
    }
}
