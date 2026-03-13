using Domain.Shared.Response;

namespace Domain.Response.CategoryResponse
{
    public class GetCategoryResponse : BaseResponse
    {
        public BaseQueryResponse<IEnumerable<object>> Result { get; set; }
    }
}
