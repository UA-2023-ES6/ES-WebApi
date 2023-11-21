﻿using Microsoft.EntityFrameworkCore;
using OneCampus.Domain.Entities.Forums;
using OneCampus.Domain.Repositories;
using OneCampus.Infrastructure.Data;
using Database = OneCampus.Infrastructure.Data.Entities;

namespace OneCampus.Infrastructure.Repositories;

public class QuestionRepository : IQuestionRepository
{
    private readonly IDbContextFactory<OneCampusDbContext> _oneCampusDbContextFactory;

    public QuestionRepository(IDbContextFactory<OneCampusDbContext> oneCampusDbContextFactory)
    {
        _oneCampusDbContextFactory = oneCampusDbContextFactory.ThrowIfNull().Value;
    }

    public async Task<Question?> CreateAsync(string content, int groupId, Guid userId)
    {
        using (var context = await _oneCampusDbContextFactory.CreateDbContextAsync())
        {
            var user = await context.Users.FindAsync(userId);
            if (user is null)
            {
                return null;
            }

            var question = new Database.Question
            {
                Content = content,
                GroupId = groupId,
                UserId = userId,
                CreateDate = DateTime.UtcNow
            };

            var result = await context.Questions.AddAsync(question);

            await context.SaveChangesAsync();

            return result.Entity.ToQuestion();
        }
    }

    public async Task<IEnumerable<Question>> GetQuestionsByGroupAsync(int groupId)
    {
        using (var context = await _oneCampusDbContextFactory.CreateDbContextAsync())
        {
            return await context.Questions
                .AsNoTracking()
                .Include(item => item.User)
                .Where(m => m.GroupId == groupId)
                .OrderBy(item => item.CreateDate)
                .Select(item => item.ToQuestion()!)
                .ToListAsync();
        }
    }
}
