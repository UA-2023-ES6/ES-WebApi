using OneCampus.Domain.Entities.Forums;

namespace OneCampus.Domain.Repositories;

public interface IQuestionRepository
{
    Task<Question?> CreateAsync(string content, int groupId, Guid userId);

    Task<IEnumerable<Question>> GetQuestionsByGroupAsync(int groupId);
}
