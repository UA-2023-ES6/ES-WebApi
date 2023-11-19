using AutoFixture.Dsl;

namespace OneCampus.Infrastructure.Tests.Helpers;

public static class QuestionHelper
{
    private static readonly Fixture _fixture = new();

    static QuestionHelper()
    {
        _fixture.Customizations.Add(new IgnoreVirtualMembers());
    }

    public static async Task<Database.Question> AddQuestionAsync(
        IDbContextFactory<OneCampusDbContext> dbContextFactory,
        int groupId,
        Guid userId)
    {
        return await AddQuestionAsync(
            dbContextFactory,
            groupId,
            userId,
            builder => builder);
    }

    public static async Task<Database.Question> AddQuestionAsync(
        IDbContextFactory<OneCampusDbContext> dbContextFactory,
        int groupId,
        Guid userId,
        Func<IPostprocessComposer<Database.Question>, IPostprocessComposer<Database.Question>> builder)
    {
        var postprocessComposer = _fixture.Build<Database.Question>()
            .Without(item => item.Id)
            .With(item => item.GroupId, groupId)
            .With(item => item.UserId, userId);

        var dbQuestion = builder(postprocessComposer)
        .Create();

        using (var dbContext = await dbContextFactory.CreateDbContextAsync())
        {
            var result = await dbContext.Questions.AddAsync(dbQuestion);

            await dbContext.SaveChangesAsync();

            return result.Entity;
        }
    }
}