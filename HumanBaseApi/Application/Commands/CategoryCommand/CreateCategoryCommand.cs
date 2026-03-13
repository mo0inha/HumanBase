using Application.Shared;
using Application.Shared.Interfaces;
using Domain.Entities;
using Domain.Request.CategoryRequest;
using Domain.Response.CategoryResponse;

namespace Application.Commands.CategoryCommand
{
    public class CreateCategoryCommand : BaseCommand<Category, CreateCategoryRequest, CreateCategoryResponse>
    {
        public CreateCategoryCommand(IRepository repository) : base(repository)
        {
        }

        protected override Task BeforeChanges(CreateCategoryRequest request)
        {
            return Task.CompletedTask;
        }

        protected async override Task<Category> Changes(CreateCategoryRequest request)
        {
            var category = new Category(request.Description, request.TypeFinancial);

            await _repository.AddAsync(category);

            return category;
        }
    }
}
