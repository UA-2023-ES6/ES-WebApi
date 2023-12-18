using OneCampus.Domain;
using OneCampus.Domain.Entities.Messages;
using OneCampus.Domain.Exceptions;
using OneCampus.Domain.Repositories;
using OneCampus.Domain.Services;

namespace OneCampus.Application.Services;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IPermissionService _permissionService;

    public MessageService(
        IMessageRepository messageRepository,
        IUserRepository userRepository,
        IGroupRepository groupRepository,
        IPermissionService permissionService)
    {
        _messageRepository = messageRepository.ThrowIfNull().Value;
        _userRepository = userRepository.ThrowIfNull().Value;
        _groupRepository = groupRepository.ThrowIfNull().Value;
        _permissionService = permissionService.ThrowIfNull().Value;
    }

    public async Task<Message?> CreateMessageAsync(int groupId, string content, Guid userId)
    {
        content.Throw()
            .IfEmpty()
            .IfWhiteSpace();

        groupId.Throw()
            .IfNegativeOrZero();

        await _permissionService.ValidatePermissionAsync(userId, groupId, PermissionType.CreateMessage);

        var user = await _userRepository.FindAsync(userId);
        if (user is null)
        {
            throw new NotFoundException("user not found");
        }

        var group = await _groupRepository.FindAsync(groupId);
        if (group is null)
        {
            throw new NotFoundException("group not found");
        }

        return await _messageRepository.CreateAsync(content, groupId, userId);
    }

    public async Task<IEnumerable<Message>> FindMessagesByGroupAsync(int groupId)
    {
        groupId.Throw()
            .IfNegativeOrZero();

        var group = await _groupRepository.FindAsync(groupId);
        if (group is null)
        {
            throw new NotFoundException("group not found");
        }

        return await _messageRepository.GetMessagesByGroupAsync(groupId);
    }
}
