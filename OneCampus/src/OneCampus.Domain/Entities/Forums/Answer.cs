namespace OneCampus.Domain.Entities.Forums;

public sealed record Answer
{
    public int Id { get; }
    public int QuestionId { get; }
    public string Content { get; }
    public string SenderName { get; }
    public DateTime CreateDate { get; }


    public Answer(int id, int questionId, string content, String senderUserName, DateTime createDate)
    {
        Id = id.Throw()
            .IfLessThan(0)
            .Value;

        QuestionId = questionId;
        Content = content;
        SenderName = senderUserName;
        CreateDate = createDate;
    }
}
