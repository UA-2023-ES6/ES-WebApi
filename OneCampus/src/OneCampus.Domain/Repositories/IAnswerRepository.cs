using OneCampus.Domain.Entities.Forums;

namespace OneCampus.Domain.Repositories;

public interface IAnswerRepository
{
    Task<Answer?> CreateAsync(string content, int questionId, Guid userId);

    Task<IEnumerable<Answer>> GetAnswersByQuestionAsync(int questionId);


}
