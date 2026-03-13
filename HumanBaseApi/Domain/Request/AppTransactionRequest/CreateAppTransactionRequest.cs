using Domain.Entities.EType;
using Domain.Response.AppTransactionResponse;
using Domain.Shared.Request;

namespace Domain.Request.AppTransactionRequest
{
    public class CreateAppTransactionsRequest : BaseRequest<CreateAppTransactionsResponse>
    {
        public string Description { get; set; }
        public decimal Value { get; set; }
        public ETypeFinancial TypeFinancial { get; set; }
        public Guid CategoryId { get; set; }
        public Guid PersonId { get; set; }
    }
}
