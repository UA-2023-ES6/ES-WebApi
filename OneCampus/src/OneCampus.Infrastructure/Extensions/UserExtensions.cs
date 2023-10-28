using Database = OneCampus.Infrastructure.Data.Entities;

namespace OneCampus.Domain.Entities.Users;

internal static class UserExtensions
{
    internal static User? ToUser(this Database.User? user)
    {
        if (user is null)
        {
            return null;
        }

        return new User(user.Id, user.Name, user.Email);
    }
}
