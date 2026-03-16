using Application.Shared;
using Application.Shared.Interfaces;
using Domain.Entities;
using Domain.Request.PersonRequest;
using Domain.Response.PersonResonse;

namespace Application.Commands.PersonCommand
{
    public class CreatePersonCommand : BaseCommand<Person, CreatePersonRequest, CreatePersonResponse>
    {
        public CreatePersonCommand(IRepository repository) : base(repository)
        {
        }

        protected async override Task BeforeChanges(CreatePersonRequest request)
        {
            var person = await _repository.SingleAsync<Person>(x => x.Name == request.Name && !x.IsDeleted);

            if (person != null && person.Name == request.Name)
            {
                _response.AddErrorAlreadyExist<Person>();
            }
        }

        protected async override Task<Person> Changes(CreatePersonRequest request)
        {
            var person = new Person(request.Name, DateTime.SpecifyKind(request.BirthDate, DateTimeKind.Utc));

            await _repository.AddAsync(person);

            return person;
        }
    }
}
