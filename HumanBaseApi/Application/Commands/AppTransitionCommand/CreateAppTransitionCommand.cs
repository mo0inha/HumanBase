using Application.Shared;
using Application.Shared.Interfaces;
using Domain.Entities;
using Domain.Entities.EType;
using Domain.Request.AppTransactionRequest;
using Domain.Response.AppTransactionResponse;

namespace Application.Commands.TransitionCommand
{
    public class CreateAppTransactionCommand : BaseCommand<AppTransaction, CreateAppTransactionsRequest, CreateAppTransactionsResponse>
    {
        private Person _person;
        private Category _category;
        public CreateAppTransactionCommand(IRepository repository) : base(repository)
        {
        }

        protected async override Task BeforeChanges(CreateAppTransactionsRequest request)
        {
            _person = await _repository.SingleAsync<Person>(x => x.Id == request.PersonId && !x.IsDeleted);

            _category = await _repository.SingleAsync<Category>(x => x.Id == request.CategoryId && !x.IsDeleted);

            if (_person == null)
            {
                _response.AddErrorNotExist<Person>();
            }

            if (_category == null)
            {
                _response.AddErrorNotExist<Category>();
            }

            if (_person.BirthDay > DateTime.Today.AddYears(-18) && request.TypeFinancial != ETypeFinancial.Expense)
            {
                _response.AddError("For users under 18 years of age, only expenses are accepted.");
            }

        }

        protected async override Task<AppTransaction> Changes(CreateAppTransactionsRequest request)
        {
            var transition = new AppTransaction(request.Description, request.Value, request.TypeFinancial, _person.Id, _category.Id);

            await _repository.AddAsync(transition);

            return transition;
        }
    }
}
