using Domain.Shared.Request;
using Domain.Shared.Response;

namespace Service.Api.Shared
{
    public interface IQueryHandler<TResponse> where TResponse : BaseResponse, new()
    {
        Task<TResponse> ExecuteAsync(BaseRequest<TResponse> request);
    }
}
