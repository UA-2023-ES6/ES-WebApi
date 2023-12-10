using AutoFixture.Dsl;

namespace OneCampus.Infrastructure.Tests.Helpers;

public static class UserHelper
{
    private static readonly Fixture _fixture = new();

    static UserHelper()
    {
        _fixture.Customizations.Add(new IgnoreVirtualMembers());
    }

    public static async Task<Database.User> AddUserAsync(IDbContextFactory<OneCampusDbContext> dbContextFactory)
    {
        return await AddUserAsync(dbContextFactory, builder => builder);
    }

    public static async Task<Database.User> AddUserAsync(
        IDbContextFactory<OneCampusDbContext> dbContextFactory,
        Func<IPostprocessComposer<Database.User>, IPostprocessComposer<Database.User>> builder)
    {
        var postprocessComposer = _fixture.Build<Database.User>()
            .Without(item => item.Id)
            .Without(item => item.DeleteDate);

        var dbUser = builder(postprocessComposer)
            .Create();

        using (var dbContext = await dbContextFactory.CreateDbContextAsync())
        {
            var result = await dbContext.Users.AddAsync(dbUser);

            await dbContext.SaveChangesAsync();

            return result.Entity;
        }
    }
}
