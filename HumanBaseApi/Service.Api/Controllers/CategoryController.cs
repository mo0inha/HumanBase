using Domain.Request.CategoryRequest;
using Domain.Response.CategoryResponse;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Service.Api.Shared;
using Service.Api.Shared.DependencyInjection;

namespace Service.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : HumanBaseController
    {
        public CategoryController(IMediator mediator, IValidationProvider validationProvider) : base(mediator, validationProvider)
        {
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request, CancellationToken cancellationToken)
        {
            return await ExecuteRequest<CreateCategoryRequest, CreateCategoryResponse>(request, cancellationToken);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategory([FromQuery] GetCategoryRequest request, [FromQuery] int pageSize, [FromQuery] int page, CancellationToken cancellationToken)
        {
            request.SetNumberRegistryPage(pageSize, page);
            return await ExecuteRequest<GetCategoryRequest, GetCategoryResponse>(request, cancellationToken);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdCategory([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var request = new GetByIdCategoryRequest();
            request.SetId(id);
            return await ExecuteRequest<GetByIdCategoryRequest, GetByIdCategoryResponse>(request, cancellationToken);
        }
    }
}
