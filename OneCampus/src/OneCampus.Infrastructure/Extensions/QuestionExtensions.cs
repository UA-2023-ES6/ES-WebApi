using Database = OneCampus.Infrastructure.Data.Entities;

namespace OneCampus.Domain.Entities.Forums;

internal static class QuestionExtensions
{
    internal static Question? ToQuestion(this Database.Question? question, String senderUserName)
    {
        if (question is null)
        {
            return null;
        }

        return new Question(question.Id, question.GroupId, question.Content, senderUserName, question.CreateDate);
    }
}
