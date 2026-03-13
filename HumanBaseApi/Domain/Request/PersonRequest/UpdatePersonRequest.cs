using Domain.Response.PersonResonse;
using Domain.Shared.Request;

namespace Domain.Request.PersonRequest
{
    public class UpdatePersonRequest : BaseRequest<UpdatePersonResponse>
    {
        public string Name { get; set; }
        public DateTime Birthdate { get; set; }
    }
}
