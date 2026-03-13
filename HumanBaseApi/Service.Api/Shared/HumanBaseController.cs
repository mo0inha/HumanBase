using Domain.Shared.Request;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Service.Api.Shared.DependencyInjection;

namespace Service.Api.Shared
{
    public abstract class HumanBaseController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IValidationProvider _validationProvider;

        protected HumanBaseController(IMediator mediator, IValidationProvider validationProvider)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _validationProvider = validationProvider;
        }

        protected async Task<IActionResult> ExecuteRequest<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
            where TRequest : BaseRequest<TResponse>
        {
            try
            {
                if (_validationProvider != null)
                {
                    var validator = _validationProvider.GetValidator<TRequest>();
                    if (validator != null)
                    {
                        var validationResult = await validator.ValidateAsync(request, cancellationToken);
                        if (!validationResult.IsValid)
                        {
                            return BadRequest(validationResult.Errors);
                        }
                    }
                }

                var response = await _mediator.Send(request, cancellationToken);
                return response != null ? Ok(response) : NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }
    }
}