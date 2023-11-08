﻿using Microsoft.EntityFrameworkCore;
using OneCampus.Domain.Entities.Users;
using OneCampus.Domain.Repositories;
using OneCampus.Infrastructure.Data;

namespace OneCampus.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbContextFactory<OneCampusDbContext> _oneCampusDbContextFactory;

    public UserRepository(IDbContextFactory<OneCampusDbContext> oneCampusDbContextFactory)
    {
        _oneCampusDbContextFactory = oneCampusDbContextFactory.ThrowIfNull().Value;
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
}
