using OneCampus.Domain.Entities.Users;

namespace OneCampus.Domain.Repositories;

public interface IUserRepository
{
    Task<User> CreateAsync(Guid id, string username, string email);
    Task<User?> FindAsync(Guid id);
    Task<User?> FindByEmailAsync(string email);
}
