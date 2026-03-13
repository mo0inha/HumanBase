using Application.Shared;
using Application.Shared.Interfaces;
using Domain.Entities;
using Domain.Request.PersonRequest;
using Domain.Response.PersonResonse;

namespace Application.Query.PersonQuery
{
    public class GetByIdPersonQuery : BaseQuery<Person, GetByIdPersonRequest, GetByIdPersonResponse>
    {
        public GetByIdPersonQuery(IRepository repository) : base(repository)
        {
        }

        protected async override Task<GetByIdPersonResponse> Query(GetByIdPersonRequest request, CancellationToken cancellationToken)
        {
            var result = _repository.AsQueryable<Person>()
                .Where(x => x.Id == request.GetId())
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.IsActive,
                    x.CreatedAt,
                    x.UpdatedAt,
                });

            var response = new GetByIdPersonResponse() { Result = result };

            return await Task.FromResult(response);
        }
    }
}
