using OneCampus.Domain.Entities.Forums;

namespace OneCampus.Domain.Services;

public interface IQuestionService
{
    Task<Question?> CreateQuestionAsync(Guid userId, int groupId, string content);

    Task<IEnumerable<Question>> FindQuestionsByGroupAsync(Guid userId, int groupId);
}
