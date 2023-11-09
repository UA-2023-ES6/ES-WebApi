using OneCampus.Domain.Entities.Messages;
using OneCampus.Domain.Exceptions;
using OneCampus.Domain.Repositories;
using OneCampus.Domain.Services;

namespace OneCampus.Application.Services;

public class MessageService : IMessageService
{
    private readonly IGroupRepository _groupRepository;
    private readonly IUserRepository _userRepository;
    private readonly IInstitutionRepository _institutionRepository;

    private readonly IMessageRepository _messageRepository;


    public MessageService(
        IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository.ThrowIfNull().Value;

    }

    public async Task<Message> CreateMessageAsync(int groupId, string content, Guid userId)
    {
        content.Throw()
            .IfEmpty()
            .IfWhiteSpace();

        groupId.Throw()
            .IfNegativeOrZero();

        return await _messageRepository.CreateAsync(content, groupId, userId);
    }

    public async Task<List<Message>> FindMessagesByGroupAsync(int groupId)
    {
        groupId.Throw()
            .IfNegativeOrZero();

        var messages = await _messageRepository.FindMessagesAsync(groupId);
        if (messages is null)
        {
            throw new NotFoundException("messages not found.");
        }

        return messages;
    }
}
