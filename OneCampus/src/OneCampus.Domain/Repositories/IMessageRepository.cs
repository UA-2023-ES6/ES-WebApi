using OneCampus.Domain.Entities.Messages;

namespace OneCampus.Domain.Repositories;

public interface IMessageRepository
{
    Task<Message> CreateAsync(string content, int groupId, Guid userId);

    Task<List<Message>> FindMessagesAsync(int groupId);


}
