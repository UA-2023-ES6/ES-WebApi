using OneCampus.Domain.Entities.Forums;
using OneCampus.Domain.Exceptions;
using OneCampus.Domain.Repositories;
using OneCampus.Domain.Services;

namespace OneCampus.Application.Services;

public class AnswerService : IAnswerService
{
    private readonly IAnswerRepository _AnswerRepository;
    private readonly IUserRepository _userRepository;
    private readonly IQuestionRepository _questionRepository;

    public AnswerService(
        IAnswerRepository AnswerRepository,
        IUserRepository userRepository,
        IQuestionRepository questionRepository)
    {
        _AnswerRepository = AnswerRepository.ThrowIfNull().Value;
        _userRepository = userRepository.ThrowIfNull().Value;
        _questionRepository = questionRepository.ThrowIfNull().Value;
    }

    public async Task<Answer?> CreateAnswerAsync(int questionId, string content, Guid userId)
    {
        content.Throw()
            .IfEmpty()
            .IfWhiteSpace();

        questionId.Throw()
            .IfNegativeOrZero();

        var user = await _userRepository.FindAsync(userId);
        if (user is null)
        {
            throw new NotFoundException("user not found");
        }

        var question = await _questionRepository.FindAsync(questionId);
        if (question is null)
        {
            throw new NotFoundException("question not found");
        }

        return await _AnswerRepository.CreateAsync(content, questionId, userId);
    }

    public async Task<IEnumerable<Answer>> FindAnswersByQuestionAsync(int questionId)
    {
        questionId.Throw()
            .IfNegativeOrZero();

        var question = await _questionRepository.FindAsync(questionId);
        if (question is null)
        {
            throw new NotFoundException("question not found");
        }

        return await _AnswerRepository.GetAnswersByQuestionAsync(questionId);
    }
}
