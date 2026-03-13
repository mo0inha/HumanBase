using Domain.Entities.EType;
using Domain.Response.CategoryResponse;
using Domain.Shared.Request;

namespace Domain.Request.CategoryRequest
{
    public class CreateCategoryRequest : BaseRequest<CreateCategoryResponse>
    {
        public string Description { get; set; }
        public ETypeFinancial TypeFinancial { get; set; }
    }
}
