using Domain.Request.PersonRequest;
using FluentValidation;

namespace Application.Validator
{
    public class CreatePersonValidator : AbstractValidator<CreatePersonRequest>
    {
        public CreatePersonValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.BirthDate).NotEmpty().LessThan(DateTime.Now).GreaterThan(DateTime.UtcNow.Date.AddYears(-150));
        }
    }
}
