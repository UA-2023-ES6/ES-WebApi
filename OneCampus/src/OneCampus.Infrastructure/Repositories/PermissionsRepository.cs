using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging.Abstractions;
using OneCampus.Domain;
using OneCampus.Domain.Entities.Permissions;
using OneCampus.Domain.Repositories;
using OneCampus.Infrastructure.Data;
using OneCampus.Infrastructure.Data.Entities;
using OneCampus.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCampus.Infrastructure.Repositories
{
    public class PermissionsRepository: IPermissionRepository
    {
        private readonly IDbContextFactory<OneCampusDbContext> _oneCampusDbContextFactory;

        public PermissionsRepository(IDbContextFactory<OneCampusDbContext> oneCampusDbContextFactory)
        {
            _oneCampusDbContextFactory = oneCampusDbContextFactory;
        }

        public async Task<UserPermissions?> AllowPermissionAsync(Guid userID, int groupID, PermissionType permission)
        {
            using (var context = _oneCampusDbContextFactory.CreateDbContext()) 
            {
                var user = await context.Users.FindAsync(userID);
                if(user == null)
                {
                    return null;
                }

                var group = await context.Groups.FindAsync(groupID);
                if(group == null)
                {
                    return null;
                }

                var userGroupPermission = new UserGroupPermissions
                {
                    UserGroup = new UserGroup
                    {
                        User = user,
                        Group = group
                    },
                    Permission = new Permission
                    {
                        Name = permission.ToString() // assim??
                    }
                };

                var res = await context.UserGroupPermissions.AddAsync(userGroupPermission);
                await context.SaveChangesAsync();

                return res.Entity.ToUserPermissions();
            }
        }

        public async Task<UserPermissions?> DeleteAsync(Guid userID, int groupID)
        {
            using (var context = _oneCampusDbContextFactory.CreateDbContext())
            {
                var user = await context.Users.FindAsync(userID);
                if (user == null)
                {
                    return null;
                }

                var group = await context.Groups.FindAsync(groupID);
                if (group == null)
                {
                    return null;
                }

                var userGroup = await context.UserGroupPermissions.FirstOrDefaultAsync(item => item.UserGroup.UserId == userID && item.UserGroup.GroupId == groupID);
                if(userGroup == null)
                {
                    return null;
                }

                //deleted date??
                await context.SaveChangesAsync();

                return userGroup.ToUserPermissions();
            }
        }

        public async Task<UserPermissions?> DenyPermissionAsync(Guid userID, int groupID, PermissionType permission)
        {
            using (var context = _oneCampusDbContextFactory.CreateDbContext())
            {
                var user = await context.Users.FindAsync(userID);
                if (user == null)
                {
                    return null;
                }

                var group = await context.Groups.FindAsync(groupID);
                if (group == null)
                {
                    return null;
                }

                var perm = await context.UserGroupPermissions
                    .FirstOrDefaultAsync(item => item.UserGroup.GroupId == groupID && item.UserGroup.UserId == userID && item.Permission.Name == permission.ToString());

                if(perm == null)
                {
                    return null;
                }

                //delted date??

                await context.SaveChangesAsync();

                return perm.ToUserPermissions();
            }
        }

        public async Task<UserPermissions?> GetPermissions(Guid userID, int groupID)
        {
            using (var context = _oneCampusDbContextFactory.CreateDbContext())
            {
                var user = await context.Users.FindAsync(userID);
                if (user == null)
                {
                    return null;
                }

                var group = await context.Groups.FindAsync(groupID);
                if (group == null)
                {
                    return null;
                }

                var perms = await context.UserGroupPermissions //find all??
                    .FirstOrDefaultAsync(item => item.UserGroup.GroupId == groupID && item.UserGroup.UserId == userID);

                if (perms == null)
                {
                    return null;
                }

                return perms.ToUserPermissions(); //verificar a extensao porque nao ta a guardar as permissoes, so userid e groupid
            }
        }
    }
}
