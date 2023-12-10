namespace OneCampus.Api.Models.Responses;

public class BaseResponse<TRequest, TResponse>

{
    public TRequest Request { get; }

    public TResponse Data { get; }

    public BaseResponse(TRequest request, TResponse data)
    {
        Request = request;
        Data = data;
    }
}
