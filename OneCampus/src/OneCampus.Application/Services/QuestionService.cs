using OneCampus.Domain.Entities.Forums;
using OneCampus.Domain.Exceptions;
using OneCampus.Domain.Repositories;
using OneCampus.Domain.Services;

namespace OneCampus.Application.Services;

public class QuestionService : IQuestionService
{
    private readonly IQuestionRepository _questionRepository;
    private readonly IUserRepository _userRepository;
    private readonly IGroupRepository _groupRepository;

    public QuestionService(
        IQuestionRepository questionRepository,
        IUserRepository userRepository,
        IGroupRepository groupRepository)
    {
        _questionRepository = questionRepository.ThrowIfNull().Value;
        _userRepository = userRepository.ThrowIfNull().Value;
        _groupRepository = groupRepository.ThrowIfNull().Value;
    }

    public async Task<Question?> CreateQuestionAsync(int groupId, string content, Guid userId)
    {
        content.Throw()
            .IfEmpty()
            .IfWhiteSpace();

        groupId.Throw()
            .IfNegativeOrZero();

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

        return await _questionRepository.CreateAsync(content, groupId, userId);
    }

    public async Task<IEnumerable<Question>> FindQuestionsByGroupAsync(int groupId)
    {
        groupId.Throw()
            .IfNegativeOrZero();

        var group = await _groupRepository.FindAsync(groupId);
        if (group is null)
        {
            throw new NotFoundException("group not found");
        }

        return await _questionRepository.GetQuestionsByGroupAsync(groupId);
    }
}
