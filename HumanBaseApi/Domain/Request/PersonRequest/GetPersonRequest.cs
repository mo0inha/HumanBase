using Domain.Response.PersonResonse;
using Domain.Shared.Request;

namespace Domain.Request.PersonRequest
{
    public class GetPersonRequest : BaseRequest<GetPersonResponse>
    {
        public string? Name { get; set; }
    }
}
