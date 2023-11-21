using Microsoft.EntityFrameworkCore;
using OneCampus.Domain.Entities.Messages;
using OneCampus.Domain.Repositories;
using OneCampus.Infrastructure.Data;
using Database = OneCampus.Infrastructure.Data.Entities;

namespace OneCampus.Infrastructure.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly IDbContextFactory<OneCampusDbContext> _oneCampusDbContextFactory;

    public MessageRepository(IDbContextFactory<OneCampusDbContext> oneCampusDbContextFactory)
    {
        _oneCampusDbContextFactory = oneCampusDbContextFactory.ThrowIfNull().Value;
    }

    public async Task<Message?> CreateAsync(string content, int groupId, Guid userId)
    {
        using (var context = await _oneCampusDbContextFactory.CreateDbContextAsync())
        {
            var user = await context.Users.FindAsync(userId);
            if (user is null)
            {
                return null;
            }

            var message = new Database.Message
            {
                Content = content,
                GroupId = groupId,
                User = user,
                CreateDate = DateTime.UtcNow
            };

            var result = await context.Messages.AddAsync(message);

            await context.SaveChangesAsync();

            return result.Entity.ToMessage(user.Username);
        }
    }

    public async Task<IEnumerable<Message>> GetMessagesByGroupAsync(int groupId)
    {
        using (var context = await _oneCampusDbContextFactory.CreateDbContextAsync())
        {
            return await context.Messages
                .AsNoTracking()
                .Include(item => item.User)
                .Where(m => m.GroupId == groupId)
                .OrderBy(item => item.CreateDate)
                .Select(item => item.ToMessage(item.User.Username)!)
                .ToListAsync();
        }
    }
}
