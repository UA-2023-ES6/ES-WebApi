using Microsoft.EntityFrameworkCore;
using OneCampus.Domain.Entities.Forums;
using OneCampus.Domain.Repositories;
using OneCampus.Infrastructure.Data;
using Database = OneCampus.Infrastructure.Data.Entities;

namespace OneCampus.Infrastructure.Repositories;

public class AnswerRepository : IAnswerRepository
{
    private readonly IDbContextFactory<OneCampusDbContext> _oneCampusDbContextFactory;

    public AnswerRepository(IDbContextFactory<OneCampusDbContext> oneCampusDbContextFactory)
    {
        _oneCampusDbContextFactory = oneCampusDbContextFactory.ThrowIfNull().Value;
    }

    public async Task<Answer?> CreateAsync(string content, int questionId, Guid userId)
    {
        using (var context = await _oneCampusDbContextFactory.CreateDbContextAsync())
        {
            var user = await context.Users.FindAsync(userId);
            if (user is null)
            {
                return null;
            }

            var answer = new Database.Answer
            {
                Content = content,
                QuestionId = questionId,
                UserId = userId,
                CreateDate = DateTime.UtcNow
            };

            var result = await context.Answers.AddAsync(answer);

            await context.SaveChangesAsync();

            return result.Entity.ToAnswer(user.Username);
        }
    }

    public async Task<IEnumerable<Answer>> GetAnswersByQuestionAsync(int questionId)
    {
        using (var context = await _oneCampusDbContextFactory.CreateDbContextAsync())
        {
            return await context.Answers
                .AsNoTracking()
                .Include(item => item.User)
                .Where(m => m.QuestionId == questionId)
                .OrderBy(item => item.CreateDate)
                .Select(item => item.ToAnswer(item.User.Username)!)
                .ToListAsync();
        }
    }
}
