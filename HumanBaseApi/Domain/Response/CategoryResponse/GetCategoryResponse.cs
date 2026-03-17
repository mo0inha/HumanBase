using Domain.Shared.Response;

namespace Domain.Response.CategoryResponse
{
    public class GetCategoryResponse : BaseResponse
    {
        public BaseQueryResponse<IEnumerable<object>> Result { get; set; }
        public CategorySummary Summary { get; set; }
    }

    public class CategorySummary
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal Total { get; set; }
    }
}
