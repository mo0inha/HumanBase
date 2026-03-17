using Application.Shared;
using Application.Shared.Interfaces;
using Domain.Entities;
using Domain.Entities.EType;
using Domain.Request.CategoryRequest;
using Domain.Response.CategoryResponse;
using Domain.Shared.Response;
using Microsoft.EntityFrameworkCore;

namespace Application.Query.CategoryQuery
{
    public class GetCategoryQuery : BaseQuery<Category, GetCategoryRequest, GetCategoryResponse>
    {
        public GetCategoryQuery(IRepository repository) : base(repository)
        {
        }

        protected async override Task<GetCategoryResponse> Query(GetCategoryRequest request, CancellationToken cancellationToken)
        {
            var filter = FilterBuilder.New<Category>();

            filter = filter.And(x => !x.IsDeleted);

            if (!string.IsNullOrEmpty(request.Description)) filter = filter.And(x => x.Description.Contains(request.Description));

            var query = _repository.AsQueryable<Category>().Where(filter);

            int totalRecords = await query.CountAsync(cancellationToken);

            var appTransiction = _repository.AsQueryable<AppTransaction>()
                .Where(x => !x.IsDeleted).GroupBy(x => 1).Select(x => new
                {
                    TotalIncome = x.Where(x => x.TypeFinancial == ETypeFinancial.Income).Sum(x => (decimal?)x.Value) ?? 0,
                    TotalExpense = x.Where(x => x.TypeFinancial == ETypeFinancial.Expense).Sum(x => (decimal?)x.Value) ?? 0
                }).FirstOrDefault();

            var summary = new CategorySummary
            {
                TotalIncome = appTransiction?.TotalIncome ?? 0,
                TotalExpense = appTransiction?.TotalExpense ?? 0,
                Total = (appTransiction?.TotalExpense ?? 0) - (appTransiction?.TotalIncome ?? 0)
            };

            var result = await query.Select(x => new
            {
                x.Id,
                x.Description,
                x.TypeFinancial,
                x.IsActive,
                x.CreatedAt,
                x.UpdatedAt
            }).OrderByDescending(x => x.CreatedAt).Skip(skip).Take(request.GetNumber()).ToListAsync(cancellationToken);

            var response = new GetCategoryResponse
            {
                Result = BaseQueryResponse<object>.CreatePaginated(result.Cast<object>(), totalRecords, request.GetPage(), request.GetNumber(), request.Description),
                Summary = summary
            };

            return response;
        }
    }
}
