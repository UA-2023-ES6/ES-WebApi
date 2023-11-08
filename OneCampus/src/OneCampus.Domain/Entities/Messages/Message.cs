namespace OneCampus.Domain.Entities.Messages;

public sealed record Message
{
    public int Id { get; }
    public int GroupId { get; }
    public string Content { get; }


    public Message(int id, int groupId, string content)
    {
        Id = id.Throw()
            .IfLessThan(0)
            .Value;

        GroupId = groupId;

        Content = content;
    }
}
