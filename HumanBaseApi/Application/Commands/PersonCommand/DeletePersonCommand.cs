using Application.Shared;
using Application.Shared.Interfaces;
using Domain.Entities;
using Domain.Request.PersonRequest;
using Domain.Response.PersonResonse;

namespace Application.Commands.PersonCommand
{
    public class DeletePersonCommand : BaseCommand<Person, DeletePersonRequest, DeletePersonResponse>
    {
        private Person _person;
        public DeletePersonCommand(IRepository repository) : base(repository)
        {
        }

        protected override async Task BeforeChanges(DeletePersonRequest request)
        {
            _person = await _repository.SingleAsync<Person>(x => x.Id == request.GetId() && !x.IsDeleted);

            if (_person == null)
            {
                _response.AddError("");
                return;
            }
        }

        protected async override Task<Person> Changes(DeletePersonRequest request)
        {
            _person.SetIsDeleted();

            return _person;
        }
    }
}
