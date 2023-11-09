using Microsoft.EntityFrameworkCore;
using OneCampus.Domain.Entities.Groups;
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

    public async Task<Message> CreateAsync(string content, int groupId, Guid userId)
    {
        using (var context = await _oneCampusDbContextFactory.CreateDbContextAsync())
        {

            var message = new Database.Message
            {
                Content = content,
                GroupId = groupId,
                UserId = userId,
                CreateDate = DateTime.UtcNow,
            };

            Console.WriteLine("comes here");
            Console.WriteLine(context.Messages.GetAsyncEnumerator());


            var result = await context.Messages.AddAsync(message);

            await context.SaveChangesAsync();

            Console.WriteLine("comes here 2");


            return result.Entity.ToMessage();

        }
    }

    public async Task<List<Message>?> FindMessagesAsync(int id)
    {
        using (var context = await _oneCampusDbContextFactory.CreateDbContextAsync())
        {
            var database_messages = await context.Messages
            .AsNoTracking()
            .Where(m => m.GroupId == id)
            .ToListAsync();

            var messages = database_messages.Select(dbMsg => dbMsg.ToMessage()).ToList();

            return messages;
        }
    }


}
