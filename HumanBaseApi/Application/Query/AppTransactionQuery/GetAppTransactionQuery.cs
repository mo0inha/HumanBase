using Application.Shared;
using Application.Shared.Interfaces;
using Domain.Entities;
using Domain.Request.AppTransactionRequest;
using Domain.Response.AppTransactionResponse;
using Domain.Shared.Response;
using Microsoft.EntityFrameworkCore;

namespace Application.Query.AppTransactionQuery
{
    public class GetAppTransactionQuery : BaseQuery<AppTransaction, GetAppTransactionRequest, GetAppTransactionResponse>
    {
        public GetAppTransactionQuery(IRepository repository) : base(repository)
        {
        }

        protected async override Task<GetAppTransactionResponse> Query(GetAppTransactionRequest request, CancellationToken cancellationToken)
        {
            var filter = FilterBuilder.New<AppTransaction>();

            filter = filter.And(x => !x.IsDeleted);

            if (!string.IsNullOrEmpty(request.Description)) filter = filter.And(x => x.Description.Contains(request.Description));

            var query = _repository.AsQueryable<AppTransaction>().Where(filter);

            int totalRecords = await query.CountAsync(cancellationToken);

            var result = await query.Select(x => new
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
            }).OrderByDescending(x => x.CreatedAt).Skip(skip).Take(request.GetNumber()).ToListAsync(cancellationToken);

            var response = new GetAppTransactionResponse
            {
                Result = BaseQueryResponse<object>.CreatePaginated(result.Cast<object>(), totalRecords, request.GetPage(), request.GetNumber(), request.Description)
            };

            return response;
        }
    }
}
