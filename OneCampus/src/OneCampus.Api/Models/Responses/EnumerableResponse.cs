namespace OneCampus.Api.Models.Responses;

public class EnumerableResponse<TRequest, TResponse> : BaseResponse<TRequest, IEnumerable<TResponse>>
{
    public int TotalResults { get; }

    public EnumerableResponse(TRequest request, IEnumerable<TResponse> data, int totalResults)
        : base(request, data)
    {
        TotalResults = totalResults;
    }
}
