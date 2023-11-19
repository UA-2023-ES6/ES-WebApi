using OneCampus.Domain.Entities.Users;

namespace OneCampus.Domain.Entities.Forums;

public sealed record Answer
{
    public int Id { get; }
    public int QuestionId { get; }
    public string Content { get; }
    public Guid UserId { get; }
    public DateTime CreateDate { get; }


    public Answer(int id, int questionId, string content, Guid userId, DateTime createDate)
    {
        Id = id.Throw()
            .IfLessThan(0)
            .Value;

        QuestionId = questionId;
        Content = content;
        UserId = userId;
        CreateDate = createDate;
    }
}
