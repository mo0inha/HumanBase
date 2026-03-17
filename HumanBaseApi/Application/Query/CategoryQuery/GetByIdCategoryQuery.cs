using Application.Shared;
using Application.Shared.Interfaces;
using Domain.Entities;
using Domain.Entities.EType;
using Domain.Request.CategoryRequest;
using Domain.Response.CategoryResponse;

namespace Application.Query.CategoryQuery
{
    public class GetByIdCategoryQuery : BaseQuery<Category, GetByIdCategoryRequest, GetByIdCategoryResponse>
    {
        public GetByIdCategoryQuery(IRepository repository) : base(repository)
        {
        }

        protected async override Task<GetByIdCategoryResponse> Query(GetByIdCategoryRequest request, CancellationToken cancellationToken)
        {
            var appTransiction = _repository.AsQueryable<AppTransaction>()
                .Where(x => x.CategoryId == request.GetId() && !x.IsDeleted)
                .GroupBy(x => 1)
                .Select(x => new
                {
                    TotalIncome = x.Where(x => x.TypeFinancial == ETypeFinancial.Income).Sum(x => (decimal?)x.Value) ?? 0,
                    TotalExpense = x.Where(x => x.TypeFinancial == ETypeFinancial.Expense).Sum(x => (decimal?)x.Value) ?? 0
                })
                .FirstOrDefault();

            var result = _repository.AsQueryable<Category>()
                .Where(x => x.Id == request.GetId())
                .Select(x => new
                {
                    x.Id,
                    x.Description,
                    x.TypeFinancial,
                    Total = x.TypeFinancial == ETypeFinancial.Income ? appTransiction != null ? appTransiction.TotalIncome : 0 : appTransiction != null ? appTransiction.TotalExpense : 0,
                    x.IsActive,
                    x.CreatedAt,
                    x.UpdatedAt
                });

            var response = new GetByIdCategoryResponse() { Result = result };

            return await Task.FromResult(response);
        }
    }
}
