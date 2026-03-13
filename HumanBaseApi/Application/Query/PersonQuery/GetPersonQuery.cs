using Application.Shared;
using Application.Shared.Interfaces;
using Domain.Entities;
using Domain.Request.PersonRequest;
using Domain.Response.PersonResonse;
using Domain.Shared.Response;
using Microsoft.EntityFrameworkCore;

namespace Application.Query.PersonQuery
{
    public class GetPersonQuery : BaseQuery<Person, GetPersonRequest, GetPersonResponse>
    {
        public GetPersonQuery(IRepository repository) : base(repository)
        {
        }

        protected async override Task<GetPersonResponse> Query(GetPersonRequest request, CancellationToken cancellationToken)
        {
            var filter = FilterBuilder.New<Person>();

            if (!string.IsNullOrEmpty(request.Name)) filter = filter.And(x => x.Name.Contains(request.Name));

            int sumRecords = await _repository.AsQueryable<Person>().Where(filter).CountAsync(cancellationToken);

            var result = _repository.AsQueryable<Person>().Where(filter)
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.IsActive,
                    x.CreatedAt,
                    x.UpdatedAt
                }).OrderByDescending(x => x.CreatedAt).Skip(skip).Take(request.GetNumber()).ToListAsync(cancellationToken);

            var response = new GetPersonResponse { Result = new BaseQueryResponse<IEnumerable<object>>(result.Result) };

            return await Task.FromResult(response);
        }
    }
}
