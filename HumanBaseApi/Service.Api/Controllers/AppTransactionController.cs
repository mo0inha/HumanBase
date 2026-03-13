using Domain.Request.AppTransactionRequest;
using Domain.Response.AppTransactionResponse;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Service.Api.Shared;
using Service.Api.Shared.DependencyInjection;

namespace Service.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppTransactionController : HumanBaseController
    {
        public AppTransactionController(IMediator mediator, IValidationProvider validationProvider) : base(mediator, validationProvider)
        {
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppTransaction([FromBody] CreateAppTransactionsRequest request, CancellationToken cancellationToken)
        {
            return await ExecuteRequest<CreateAppTransactionsRequest, CreateAppTransactionsResponse>(request, cancellationToken);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAppTransaction([FromQuery] GetAppTransactionRequest request, [FromQuery] int pageSize, [FromQuery] int page, CancellationToken cancellationToken)
        {
            request.SetNumberRegistryPage(pageSize, page);
            return await ExecuteRequest<GetAppTransactionRequest, GetAppTransactionResponse>(request, cancellationToken);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAppTransaction([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var request = new GetByIdAppTransactionRequest();
            request.SetId(id);
            return await ExecuteRequest<GetByIdAppTransactionRequest, GetByIdAppTransactionResponse>(request, cancellationToken);
        }
    }
}
