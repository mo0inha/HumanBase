using Application.Shared;
using Application.Shared.Interfaces;
using Domain.Entities;
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
                Result = BaseQueryResponse<object>.CreatePaginated(result.Cast<object>(), totalRecords, request.GetPage(), request.GetNumber(), request.Description)
            };

            return response;
        }
    }
}
