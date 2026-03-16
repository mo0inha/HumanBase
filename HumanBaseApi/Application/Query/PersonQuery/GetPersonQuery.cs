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

        protected override async Task<GetPersonResponse> Query(GetPersonRequest request, CancellationToken cancellationToken)
        {
            var filter = FilterBuilder.New<Person>();

            filter = filter.And(x => !x.IsDeleted);

            if (!string.IsNullOrEmpty(request.Name)) filter = filter.And(x => x.Name.Contains(request.Name));

            var query = _repository.AsQueryable<Person>().Where(filter);

            int totalRecords = await query.CountAsync(cancellationToken);

            var result = await query.Select(x => new
            {
                x.Id,
                x.Name,
                x.BirthDate,
                x.IsActive,
                x.CreatedAt,
                x.UpdatedAt
            }).OrderByDescending(x => x.CreatedAt).Skip(skip).Take(request.GetNumber()).ToListAsync(cancellationToken);

            var response = new GetPersonResponse
            {
                Result = BaseQueryResponse<object>.CreatePaginated(result.Cast<object>(), totalRecords, request.GetPage(), request.GetNumber(), request.Name)
            };

            return response;
        }
    }
}
