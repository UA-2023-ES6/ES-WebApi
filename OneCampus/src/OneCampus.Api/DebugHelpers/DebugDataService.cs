using Microsoft.EntityFrameworkCore;
using OneCampus.Domain.Services;
using OneCampus.Infrastructure.Data;
using Database = OneCampus.Infrastructure.Data.Entities;

namespace OneCampus.Api;

public class DebugDataService
{
    private const int DefaultInstitutionId = 1;
    private readonly Guid RootUser = Guid.Parse("00000000-0000-0000-0000-000000000001");

    private readonly IDbContextFactory<OneCampusDbContext> _dbContextFactory;
    private readonly IUsersService _usersService;
    private readonly IGroupService _groupService;

    public DebugDataService(
        IDbContextFactory<OneCampusDbContext> dbContextFactory,
        IUsersService usersService,
        IGroupService groupService)
    {
        _dbContextFactory = dbContextFactory;
        _usersService = usersService;
        _groupService = groupService;
    }

    internal async Task CreateDefaultDataAsync()
    {
        await AddUserAsync(RootUser, "User 1", "user1@OneCampus.pt");
        await AddInstitutionAsync(RootUser, "Default Institution", DefaultInstitutionId);

        var user = await AddUserAsync(Guid.Parse("00000000-0000-0000-0000-000000000002"), "User 2", "user2@OneCampus.pt");
        await _groupService.AddUserAsync(RootUser, DefaultInstitutionId, user.Id);
        user = await AddUserAsync(Guid.Parse("00000000-0000-0000-0000-000000000003"), "User 3", "user3@OneCampus.pt");
        await _groupService.AddUserAsync(RootUser, DefaultInstitutionId, user.Id);
        user = await AddUserAsync(Guid.Parse("00000000-0000-0000-0000-000000000004"), "User 4", "user4@OneCampus.pt");
        await _groupService.AddUserAsync(RootUser, DefaultInstitutionId, user.Id);
    }

    internal async Task AddUserToDefaultInstitution(Guid userId)
    {
        await _groupService.AddUserAsync(RootUser, DefaultInstitutionId, userId);
    }

    internal async Task<Database.Institution> AddInstitutionAsync(Guid userId, string name, int? id = null)
    {
        using (var context = await _dbContextFactory.CreateDbContextAsync())
        {
            Database.Institution? institution = null;
            if (id.HasValue)
            {
                institution = await context.Institutions.FindAsync(id.Value);
                if (institution is not null)
                {
                    return institution;
                }
            }

            var user = await context.Users.FindAsync(userId);

            var defaultGroup = new Database.Group
            {
                Name = name,
                CreateDate = DateTime.UtcNow,
                Users = new List<Database.User>{ user! }
            };

            var result = await context.Institutions
                .AddAsync(new Database.Institution
                {
                    Id = id ?? 0,
                    Name = name,
                    CreateDate = DateTime.UtcNow,
                    Group = defaultGroup
                });

            await context.SaveChangesAsync();

            return result.Entity;
        }
    }

    internal async Task<Domain.Entities.Users.User> AddUserAsync(Guid id, string username, string email)
    {
        var user = await  _usersService.FindAsync(id);
        if (user is not null)
        {
            return user;
        }

        return await _usersService.CreateAsync(id, username, email);
    }
}
