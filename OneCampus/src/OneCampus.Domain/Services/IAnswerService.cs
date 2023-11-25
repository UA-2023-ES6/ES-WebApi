using OneCampus.Domain.Entities.Forums;

namespace OneCampus.Domain.Services;

public interface IAnswerService
{
    Task<Answer?> CreateAnswerAsync(int questionId, string content, Guid userId);

    Task<IEnumerable<Answer>> FindAnswersByQuestionAsync(int questionId);


}

