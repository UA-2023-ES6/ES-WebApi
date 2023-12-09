using OneCampus.Domain.Entities.Users;

namespace OneCampus.Domain.Entities.Forums;

public sealed record Question
{
    public int Id { get; }
    public int GroupId { get; }
    public string Content { get; }
    public string SenderName { get; }
    public DateTime CreateDate { get; }


    public Question(int id, int groupId, string content, String senderUserName, DateTime createDate)
    {
        Id = id.Throw()
            .IfLessThan(0)
            .Value;

        GroupId = groupId;
        Content = content;
        SenderName = senderUserName;
        CreateDate = createDate;
    }
}
