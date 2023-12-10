using Microsoft.EntityFrameworkCore;
using OneCampus.Domain.Entities.Users;
using OneCampus.Domain.Repositories;
using OneCampus.Infrastructure.Data;
using Database = OneCampus.Infrastructure.Data.Entities;

namespace OneCampus.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbContextFactory<OneCampusDbContext> _oneCampusDbContextFactory;

    public UserRepository(IDbContextFactory<OneCampusDbContext> oneCampusDbContextFactory)
    {
        _oneCampusDbContextFactory = oneCampusDbContextFactory.ThrowIfNull().Value;
    }

    public async Task<User> CreateAsync(Guid id, string username, string email)
    {
        using (var context = await _oneCampusDbContextFactory.CreateDbContextAsync())
        {
            var result = await context.Users
                .AddAsync(new Database.User
                {
                    Id = id,
                    Username = username,
                    Email = email,
                    CreateDate = DateTime.UtcNow
                });

            await context.SaveChangesAsync();

            return result.Entity.ToUser()!;
        }
    }

    public async Task<User?> FindAsync(Guid id)
    {
        using (var context = await _oneCampusDbContextFactory.CreateDbContextAsync())
        {
            var user = await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.DeleteDate == null && item.Id == id);

            return user.ToUser();
        }
    }

    public async Task<User?> FindByEmailAsync(string email)
    {
        using (var context = await _oneCampusDbContextFactory.CreateDbContextAsync())
        {
            var user = await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.DeleteDate == null && item.Email == email);

            return user.ToUser();
        }
    }
}
