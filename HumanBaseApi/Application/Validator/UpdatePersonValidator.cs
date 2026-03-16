using Domain.Request.PersonRequest;
using FluentValidation;

namespace Application.Validator
{
    public class UpdatePersonValidator : AbstractValidator<CreatePersonRequest>
    {
        public UpdatePersonValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.BirthDate).NotEmpty().LessThan(DateTime.Now).GreaterThan(DateTime.MinValue);
        }
    }
}
