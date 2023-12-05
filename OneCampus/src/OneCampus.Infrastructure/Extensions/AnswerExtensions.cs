using Database = OneCampus.Infrastructure.Data.Entities;

namespace OneCampus.Domain.Entities.Forums;

internal static class AnswerExtensions
{
    internal static Answer? ToAnswer(this Database.Answer? Answer, String senderUserName)
    {
        if (Answer is null)
        {
            return null;
        }

        return new Answer(Answer.Id, Answer.QuestionId, Answer.Content, senderUserName, Answer.CreateDate);
    }

}
