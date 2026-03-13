using Domain.Response.PersonResonse;
using Domain.Shared.Request;

namespace Domain.Request.PersonRequest
{
    public class CreatePersonRequest : BaseRequest<CreatePersonResponse>
    {
        public string Name { get; set; }
        public DateTime BirthDay { get; set; }
    }
}
