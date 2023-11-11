using OneCampus.Domain.Entities.Messages;

namespace OneCampus.Domain.Repositories;

public interface IMessageRepository
{
    Task<Message?> CreateAsync(string content, int groupId, Guid userId);

    Task<IEnumerable<Message>> GetMessagesByGroupAsync(int groupId);
}
