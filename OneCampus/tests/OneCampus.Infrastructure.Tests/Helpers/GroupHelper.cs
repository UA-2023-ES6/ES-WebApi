using AutoFixture.Dsl;

namespace OneCampus.Infrastructure.Tests.Helpers;

public static class GroupHelper
{
    private static readonly Fixture _fixture = new();

    static GroupHelper()
    {
        _fixture.Customizations.Add(new IgnoreVirtualMembers());
    }

    public static async Task<Database.Group> AddGroupWithInstitutionAsync(
        IDbContextFactory<OneCampusDbContext> dbContextFactory)
    {
        return await AddGroupWithInstitutionAsync(
            dbContextFactory,
            builder => builder.Without(item => item.DeleteDate));
    }

    public static async Task<Database.Group> AddGroupWithInstitutionAsync(
        IDbContextFactory<OneCampusDbContext> dbContextFactory,
        Func<IPostprocessComposer<Database.Group>, IPostprocessComposer<Database.Group>> builder)
    {
        var postprocessComposer = GetMockedGroupWithInstitution();

        var dbGroupWithInstitution = builder(postprocessComposer)
            .Create();

        using (var dbContext = await dbContextFactory.CreateDbContextAsync())
        {
            var result = await dbContext.Groups.AddAsync(dbGroupWithInstitution);

            await dbContext.SaveChangesAsync();

            return result.Entity;
        }
    }

    public static async Task<Database.Group> AddGroupAsync(IDbContextFactory<OneCampusDbContext> dbContextFactory, int parentId)
    {
        return await AddGroupAsync(
            dbContextFactory,
            builder => builder.With(item => item.ParentId, parentId));
    }

    public static async Task<Database.Group> AddGroupAsync(
        IDbContextFactory<OneCampusDbContext> dbContextFactory,
        Func<IPostprocessComposer<Database.Group>, IPostprocessComposer<Database.Group>> builder)
    {
        var postprocessComposer = _fixture.Build<Database.Group>()
            .Without(item => item.ParentId)
            .Without(item => item.Institution)
            .Without(item => item.DeleteDate);

        var dbGroupWithInstitution = builder(postprocessComposer)
            .Create();

        using (var dbContext = await dbContextFactory.CreateDbContextAsync())
        {
            var result = await dbContext.Groups.AddAsync(dbGroupWithInstitution);

            await dbContext.SaveChangesAsync();

            return result.Entity;
        }
    }

    public static async Task AddUsersToGroupAsync(
    IDbContextFactory<OneCampusDbContext> dbContextFactory,
    int groupId,
    params Guid[] usersIds)
    {
        using (var dbContext = await dbContextFactory.CreateDbContextAsync())
        {
            var group = await dbContext.Groups
                .Include(item => item.Users)
                .FirstAsync(item => item.Id == groupId);

            foreach (var userId in usersIds)
            {
                var user = await dbContext.Users.FindAsync(userId);

                group.Users.Add(user!);
            }

            await dbContext.SaveChangesAsync();
        }
    }

    #region Private Methods

    private static IPostprocessComposer<Database.Group> GetMockedGroupWithInstitution()
    {
        var institution = _fixture.Build<Database.Institution>()
            .Without(item => item.Group)
            .Without(item => item.DeleteDate)
            .Without(item => item.GroupId)
            .Create();

        return _fixture.Build<Database.Group>()
            .Without(item => item.ParentId)
            .With(item => item.Institution, institution);
    }

    #endregion
}