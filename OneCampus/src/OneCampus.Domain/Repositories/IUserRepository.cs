using OneCampus.Domain.Entities.Users;

namespace OneCampus.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> FindAsync(Guid id);
}
