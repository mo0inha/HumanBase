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

            if (!string.IsNullOrEmpty(request.Description)) filter = filter.And(x => x.Description.Contains(request.Description));

            int sumRecords = await _repository.AsQueryable<Category>().Where(filter).CountAsync(cancellationToken);

            var result = _repository.AsQueryable<Category>().Where(filter)
                .Select(x => new
                {
                    x.Id,
                    x.Description,
                    x.TypeFinancial,
                    x.IsActive,
                    x.CreatedAt,
                    x.UpdatedAt
                }).OrderByDescending(x => x.CreatedAt).Skip(skip).Take(request.GetNumber()).ToListAsync(cancellationToken);

            var response = new GetCategoryResponse { Result = new BaseQueryResponse<IEnumerable<object>>(result.Result) };

            return await Task.FromResult(response);
        }
    }
}
