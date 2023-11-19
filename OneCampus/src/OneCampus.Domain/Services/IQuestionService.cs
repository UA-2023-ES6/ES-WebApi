using OneCampus.Domain.Entities.Forums;

namespace OneCampus.Domain.Services;

public interface IQuestionService
{
    Task<Question?> CreateQuestionAsync(int groupId, string content, Guid userId);

    Task<IEnumerable<Question>> FindQuestionsByGroupAsync(int groupId);


}

