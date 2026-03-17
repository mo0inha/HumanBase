using Application.Shared;
using Application.Shared.Interfaces;
using Domain.Entities;
using Domain.Request.AppTransactionRequest;
using Domain.Response.AppTransactionResponse;

namespace Application.Query.AppTransactionQuery
{
    public class GetByIdAppTransactionQuery : BaseQuery<AppTransaction, GetByIdAppTransactionRequest, GetByIdAppTransactionResponse>
    {
        public GetByIdAppTransactionQuery(IRepository repository) : base(repository)
        {
        }

        protected async override Task<GetByIdAppTransactionResponse> Query(GetByIdAppTransactionRequest request, CancellationToken cancellationToken)
        {
            var result = _repository.AsQueryable<AppTransaction>()
                .Where(x => x.Id == request.GetId())
                .Select(x => new
                {
                    x.Id,
                    x.Description,
                    x.Value,
                    x.TypeFinancial,
                    x.CategoryId,
                    x.PersonId,
                    x.IsActive,
                    x.CreatedAt,
                    x.UpdatedAt
                });

            var response = new GetByIdAppTransactionResponse() { Result = result };

            return await Task.FromResult(response);
        }
    }
}
