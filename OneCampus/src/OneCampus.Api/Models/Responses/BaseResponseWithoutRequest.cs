namespace OneCampus.Api.Models.Responses;

public class BaseResponseWithoutRequest<TResponse>
{
    public TResponse Data { get; }

    public BaseResponseWithoutRequest(TResponse data)
    {
        Data = data;
    }
}
