using Application.Shared;
using Application.Shared.Interfaces;
using Domain.Entities;
using Domain.Entities.EType;
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
            var appTransiction = _repository.AsQueryable<AppTransaction>()
                .Where(x => x.PersonId == request.GetId() && !x.IsDeleted)
                .GroupBy(x => 1)
                .Select(x => new
                {
                    TotalIncome = x.Where(x => x.TypeFinancial == ETypeFinancial.Income).Sum(x => (decimal?)x.Value) ?? 0,
                    TotalExpense = x.Where(x => x.TypeFinancial == ETypeFinancial.Expense).Sum(x => (decimal?)x.Value) ?? 0
                })
                .FirstOrDefault();

            var result = _repository.AsQueryable<Person>()
                .Where(x => x.Id == request.GetId())
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    TotalExpense = appTransiction != null ? appTransiction.TotalExpense : 0,
                    TotalIncome = appTransiction != null ? appTransiction.TotalIncome : 0,
                    Total = (appTransiction != null ? appTransiction.TotalExpense : 0) - (appTransiction != null ? appTransiction.TotalIncome : 0),
                    x.IsActive,
                    x.CreatedAt,
                    x.UpdatedAt,
                });

            var response = new GetByIdPersonResponse() { Result = result };

            return await Task.FromResult(response);
        }
    }
}
