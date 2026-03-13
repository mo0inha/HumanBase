using Application.Shared.Interfaces;
using Domain.Shared.Request;
using Domain.Shared.Response;
using MediatR;

namespace Application.Shared
{
    public abstract class BaseCommand<TEntity, TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : BaseRequest<TResponse> where TResponse : BaseResponse where TEntity : class
    {
        protected readonly IRepository _repository;
        protected readonly TResponse _response;

        protected BaseCommand(IRepository repository)
        {
            _repository = repository;
            _response = Activator.CreateInstance<TResponse>();
        }

        protected abstract Task BeforeChanges(TRequest request);

        protected abstract Task<TEntity> Changes(TRequest request);

        public async Task<TResponse> ExecuteAsync(TRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                await BeforeChanges(request);

                if (_response.HasErrors)
                    return _response;

                var entity = await Changes(request);

                if (entity != null)
                {
                    await _repository.SaveChangesAsync();

                    _response.Id = entity.GetType().GetProperty("Id")?.GetValue(entity) as Guid?;
                    _response.Message = "Operação realizada com sucesso.";
                    _response.StatusCode = 200;
                }

                return _response;
            }
            catch (Exception ex)
            {
                _response.SetError(ex.Message, 500);
                return _response;
            }
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
        {
            return await ExecuteAsync(request, cancellationToken);
        }
    }
}
