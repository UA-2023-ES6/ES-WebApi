namespace OneCampus.Api.Models.Requests;

public class CreateAnswerRequest
{
    public string Content { get; set; } = null!;

    public int QuestionId { get; set; }

    public Guid UserId { get; set; }
}

