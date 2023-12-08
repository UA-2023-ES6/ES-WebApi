using OneCampus.Domain.Entities.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database = OneCampus.Infrastructure.Data.Entities;

namespace OneCampus.Infrastructure.Extensions
{
    internal static class PermissionExtentions
    {
        internal static UserPermissions? ToUserPermissions(this Database.UserGroupPermissions? userGroupPermissions)
        {
            if (userGroupPermissions is null)
            {
                return null;
            }

            return new UserPermissions(userGroupPermissions.UserGroup.UserId, userGroupPermissions.UserGroup.GroupId);
        }
    }
}
