using FluentValidation;

namespace Service.Api.Shared.DependencyInjection
{
    public interface IValidationProvider
    {
        IValidator<T> GetValidator<T>();
    }
}
