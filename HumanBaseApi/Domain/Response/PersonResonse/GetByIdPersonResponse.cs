using Domain.Shared.Response;

namespace Domain.Response.PersonResonse
{
    public class GetByIdPersonResponse : BaseResponse
    {
        public IEnumerable<object> Result { get; set; }
    }
}
