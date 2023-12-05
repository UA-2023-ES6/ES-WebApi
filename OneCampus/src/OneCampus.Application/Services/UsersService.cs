using OneCampus.Domain.Entities.Users;
using OneCampus.Domain.Repositories;
using OneCampus.Domain.Services;

namespace OneCampus.Application.Services;

public class UsersService : IUsersService
{
    private readonly IUserRepository _userRepository;

    public UsersService(IUserRepository userRepository)
    {
        _userRepository = userRepository.ThrowIfNull().Value;
    }

    public async Task<User> CreateAsync(Guid id, string userName, string email)
    {
        id.Throw()
            .IfDefault();

        userName.Throw()
            .IfEmpty()
            .IfWhiteSpace();

        email.Throw()
            .IfEmpty()
            .IfWhiteSpace();

        return await _userRepository.CreateAsync(id, userName, email);
    }

    public async Task<User?> FindAsync(Guid id)
    {
        id.Throw()
            .IfDefault();

        return await _userRepository.FindAsync(id);
    }

    public async Task<User?> FindByEmailAsync(string email)
    {
        email.Throw()
            .IfEmpty()
            .IfWhiteSpace();

        return await _userRepository.FindByEmailAsync(email);
    }
}