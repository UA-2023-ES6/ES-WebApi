using OneCampus.Domain.Entities.Forums;

namespace OneCampus.Domain.Services;

public interface IAnswerService
{
    Task<Answer?> CreateAnswerAsync(Guid userId, int questionId, string content);

    Task<IEnumerable<Answer>> FindAnswersByQuestionAsync(Guid userId, int questionId);


}

