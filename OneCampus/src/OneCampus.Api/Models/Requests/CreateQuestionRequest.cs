namespace OneCampus.Api.Models.Requests;

public class CreateQuestionRequest
{
    public string Content { get; set; } = null!;

    public int GroupId { get; set; }
}

