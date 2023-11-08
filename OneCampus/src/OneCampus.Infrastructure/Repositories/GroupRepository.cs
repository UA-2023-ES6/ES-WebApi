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

    public async Task<Group?> FindAsync(int id)
    {
        using (var context = await _oneCampusDbContextFactory.CreateDbContextAsync())
        {
            var group = await context.Groups
                .AsNoTracking()
                .Include(item => item.Users)
                .FirstOrDefaultAsync(item => item.DeleteDate == null && item.Id == id);

            return group.ToGroup();
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

    public async Task<IEnumerable<Group>> GetSubGroupsAsync(int id)
    {
        using (var context = await _oneCampusDbContextFactory.CreateDbContextAsync())
        {
            var groups = await context.Groups
                .AsNoTracking()
                .Where(item => item.DeleteDate == null && item.ParentId == id)
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

    public async Task<Group?> AddUserAsync(int groupId, Guid userId)
    {
        using (var context = await _oneCampusDbContextFactory.CreateDbContextAsync())
        {
            var group = await context.Groups
                .Include(item => item.Users)
                .FirstOrDefaultAsync(item => item.DeleteDate == null && item.Id == groupId);
            if (group == null)
            {
                return null;
            }

            var newUser = await context.Users.FirstOrDefaultAsync(item => item.DeleteDate == null && item.Id == userId);
            if (newUser is not null)
            {
                group.Users.Add(newUser);

                await context.SaveChangesAsync();
            }

            return group.ToGroup(group.Users.Select(item => item.ToUser()!))!;
        }
    }

    public async Task<Group?> RemoveUserAsync(int groupId, Guid userId)
    {
        using (var context = await _oneCampusDbContextFactory.CreateDbContextAsync())
        {
            var group = await context.Groups
                .Include(item => item.Users)
                .FirstOrDefaultAsync(item => item.DeleteDate == null && item.Id == groupId);
            if (group == null)
            {
                return null;
            }

            var userToRemove = group.Users.FirstOrDefault(item => item.Id == userId);
            if (userToRemove is not null)
            {
                group.Users.Remove(userToRemove);

                await context.SaveChangesAsync();
            }

            return group.ToGroup(group.Users.Select(item => item.ToUser()!))!;
        }
    }
}