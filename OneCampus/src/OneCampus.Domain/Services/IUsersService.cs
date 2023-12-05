using OneCampus.Domain.Entities.Users;

namespace OneCampus.Domain.Services;

public interface IUsersService
{
    Task<User> CreateAsync(Guid id, string userName, string email);
    Task<User?> FindAsync(Guid id);
    Task<User?> FindByEmailAsync(string email);    
}