using Domain.Response.AppTransactionResponse;
using Domain.Shared.Request;

namespace Domain.Request.AppTransactionRequest
{
    public class GetAppTransactionRequest : BaseRequest<GetAppTransactionResponse>
    {
        public string? Description { get; set; }
    }
}
