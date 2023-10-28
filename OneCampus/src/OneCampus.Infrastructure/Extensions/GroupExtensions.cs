using Database = OneCampus.Infrastructure.Data.Entities;

namespace OneCampus.Domain.Entities.Groups;

internal static class GroupExtensions
{
    internal static Group? ToGroup(this Database.Group? group)
    {
        if (group is null)
        {
            return null;
        }

        return new Group(group.Id, group.Name);
    }

    internal static Group? ToGroup(this Database.Group? group, IEnumerable<User> users)
    {
        if (group is null)
        {
            return null;
        }

        return new Group(group.Id, group.Name, users.ToList());
    }

    internal static User? ToUser(this Database.User? user)
    {
        if (user is null)
        {
            return null;
        }

        return new User(user.Id, user.Name, user.Email);
    }
}
