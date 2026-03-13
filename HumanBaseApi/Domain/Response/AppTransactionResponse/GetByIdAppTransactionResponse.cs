using Domain.Shared.Response;

namespace Domain.Response.AppTransactionResponse
{
    public class GetByIdAppTransactionResponse : BaseResponse
    {
        public IEnumerable<object> Result { get; set; }
    }
}
