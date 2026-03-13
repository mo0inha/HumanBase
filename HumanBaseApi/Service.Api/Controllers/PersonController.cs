using Domain.Request.PersonRequest;
using Domain.Response.PersonResonse;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Service.Api.Shared;
using Service.Api.Shared.DependencyInjection;

namespace Service.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : HumanBaseController
    {
        public PersonController(IMediator mediator, IValidationProvider validationProvider) : base(mediator, validationProvider)
        {
        }

        [HttpPost]
        public async Task<IActionResult> CreatePerson([FromBody] CreatePersonRequest request, CancellationToken cancellationToken)
        {
            return await ExecuteRequest<CreatePersonRequest, CreatePersonResponse>(request, cancellationToken);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePerson([FromRoute] Guid id, [FromBody] UpdatePersonRequest request, CancellationToken cancellationToken)
        {
            request.SetId(id);
            return await ExecuteRequest<UpdatePersonRequest, UpdatePersonResponse>(request, cancellationToken);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var request = new DeletePersonRequest();
            request.SetId(id);
            return await ExecuteRequest<DeletePersonRequest, DeletePersonResponse>(request, cancellationToken);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPerson([FromQuery] GetPersonRequest request, [FromQuery] int pageSize, [FromQuery] int page, CancellationToken cancellationToken)
        {
            request.SetNumberRegistryPage(pageSize, page);
            return await ExecuteRequest<GetPersonRequest, GetPersonResponse>(request, cancellationToken);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdPerson([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var request = new GetByIdPersonRequest();
            request.SetId(id);
            return await ExecuteRequest<GetByIdPersonRequest, GetByIdPersonResponse>(request, cancellationToken);
        }
    }
}