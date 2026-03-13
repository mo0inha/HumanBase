using Domain.Shared.Response;

namespace Domain.Response.AppTransactionResponse
{
    public class GetAppTransactionResponse : BaseResponse
    {
        public BaseQueryResponse<IEnumerable<object>> Result { get; set; }
    }
}
