namespace OneCampus.Api.Models.Requests;

public class CreateMessageRequest
{
    public string Content { get; set; } = null!;

    public int GroupId { get; set; }

    public Guid UserId { get; set; }
}

