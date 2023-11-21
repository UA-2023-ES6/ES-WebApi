using AutoFixture.Dsl;

namespace OneCampus.Infrastructure.Tests.Helpers;

public static class MessageHelper
{
    private static readonly Fixture _fixture = new();

    static MessageHelper()
    {
        _fixture.Customizations.Add(new IgnoreVirtualMembers());
    }

    public static async Task<Database.Message> AddMessageAsync(
        IDbContextFactory<OneCampusDbContext> dbContextFactory,
        int groupId,
        Guid userId)
    {
        return await AddMessageAsync(
            dbContextFactory,
            groupId,
            userId,
            builder => builder);
    }

    public static async Task<Database.Message> AddMessageAsync(
        IDbContextFactory<OneCampusDbContext> dbContextFactory,
        int groupId,
        Guid userId,
        Func<IPostprocessComposer<Database.Message>, IPostprocessComposer<Database.Message>> builder)
    {
        var postprocessComposer = _fixture.Build<Database.Message>()
            .Without(item => item.Id)
            .With(item => item.GroupId, groupId)
            .With(item => item.UserId, userId);

        var dbMessage = builder(postprocessComposer)
        .Create();

        using (var dbContext = await dbContextFactory.CreateDbContextAsync())
        {
            var result = await dbContext.Messages.AddAsync(dbMessage);

            await dbContext.SaveChangesAsync();

            return result.Entity;
        }
    }
}