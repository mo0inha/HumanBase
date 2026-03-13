using Application.Shared;
using Application.Shared.Interfaces;
using Domain.Entities;
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
            var result = _repository.AsQueryable<Category>()
                .Where(x => x.Id == request.GetId())
                .Select(x => new
                {
                    x.Id,
                    x.Description,
                    x.TypeFinancial,
                    x.IsActive,
                    x.CreatedAt,
                    x.UpdatedAt
                });

            var response = new GetByIdCategoryResponse() { Result = result };

            return await Task.FromResult(response);
        }
    }
}
