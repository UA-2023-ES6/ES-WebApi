namespace OneCampus.Domain.Entities.Messages;

public sealed record Message
{
    public int Id { get; }
    public int GroupId { get; }
    public string Content { get; }
    public string SenderName { get; }
    public DateTime CreateDate { get; }

    public Message(int id, int groupId, string content, string senderUserName, DateTime createDate)
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
