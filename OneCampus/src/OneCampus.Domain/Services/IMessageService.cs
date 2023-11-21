using OneCampus.Domain.Entities.Messages;

namespace OneCampus.Domain.Services;

public interface IMessageService
{   
    Task<Message?> CreateMessageAsync(int groupId, string content, Guid userId);

    Task<IEnumerable<Message>> FindMessagesByGroupAsync(int groupId);


}

