using Application.Shared;
using Application.Shared.Interfaces;
using Domain.Entities;
using Domain.Request.PersonRequest;
using Domain.Response.PersonResonse;

namespace Application.Commands.PersonCommand
{
    public class UpdatePersonCommand : BaseCommand<Person, UpdatePersonRequest, UpdatePersonResponse>
    {
        private Person _person;
        public UpdatePersonCommand(IRepository repository) : base(repository)
        {
        }

        protected override async Task BeforeChanges(UpdatePersonRequest request)
        {
            _person = await _repository.SingleAsync<Person>(x => x.Id == request.GetId() && !x.IsDeleted);

            if (_person == null)
            {
                _response.AddError("");
                return;
            }
        }

        protected async override Task<Person> Changes(UpdatePersonRequest request)
        {
            _person.Name = request.Name;
            _person.BirthDate = request.Birthdate;

            return _person;
        }
    }
}
