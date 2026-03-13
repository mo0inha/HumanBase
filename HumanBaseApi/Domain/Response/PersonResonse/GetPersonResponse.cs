using Domain.Shared.Response;

namespace Domain.Response.PersonResonse
{
    public class GetPersonResponse : BaseResponse
    {
        public BaseQueryResponse<IEnumerable<object>> Result { get; set; }
    }
}
