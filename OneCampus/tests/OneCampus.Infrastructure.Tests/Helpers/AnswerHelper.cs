using AutoFixture.Dsl;

namespace OneCampus.Infrastructure.Tests.Helpers;

public static class AnswerHelper
{
    private static readonly Fixture _fixture = new();

    static AnswerHelper()
    {
        _fixture.Customizations.Add(new IgnoreVirtualMembers());
    }

    public static async Task<Database.Answer> AddAnswerAsync(
        IDbContextFactory<OneCampusDbContext> dbContextFactory,
        int questionId,
        Guid userId)
    {
        return await AddAnswerAsync(
            dbContextFactory,
            questionId,
            userId,
            builder => builder);
    }

    public static async Task<Database.Answer> AddAnswerAsync(
        IDbContextFactory<OneCampusDbContext> dbContextFactory,
        int questionId,
        Guid userId,
        Func<IPostprocessComposer<Database.Answer>, IPostprocessComposer<Database.Answer>> builder)
    {
        var postprocessComposer = _fixture.Build<Database.Answer>()
            .Without(item => item.Id)
            .With(item => item.QuestionId, questionId)
            .With(item => item.UserId, userId);

        var dbAnswer = builder(postprocessComposer)
        .Create();

        using (var dbContext = await dbContextFactory.CreateDbContextAsync())
        {
            var result = await dbContext.Answers.AddAsync(dbAnswer);

            await dbContext.SaveChangesAsync();

            return result.Entity;
        }
    }
}