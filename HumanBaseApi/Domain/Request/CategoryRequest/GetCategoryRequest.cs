using Domain.Response.CategoryResponse;
using Domain.Shared.Request;

namespace Domain.Request.CategoryRequest
{
    public class GetCategoryRequest : BaseRequest<GetCategoryResponse>
    {
        public string? Description { get; set; }
    }
}
