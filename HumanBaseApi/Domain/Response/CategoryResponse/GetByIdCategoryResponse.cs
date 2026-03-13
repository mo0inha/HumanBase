using Domain.Shared.Response;

namespace Domain.Response.CategoryResponse
{
    public class GetByIdCategoryResponse : BaseResponse
    {
        public IEnumerable<object> Result { get; set; }
    }
}
