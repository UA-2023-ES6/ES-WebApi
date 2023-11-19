using OneCampus.Domain.Entities.Users;

namespace OneCampus.Domain.Entities.Forums;

public sealed record Question
{
    public int Id { get; }
    public int GroupId { get; }
    public string Content { get; }
    public Guid UserId { get; }
    public DateTime CreateDate { get; }


    public Question(int id, int groupId, string content, Guid userId, DateTime createDate)
    {
        Id = id.Throw()
            .IfLessThan(0)
            .Value;

        GroupId = groupId;
        Content = content;
        UserId = userId;
        CreateDate = createDate;
    }
}
