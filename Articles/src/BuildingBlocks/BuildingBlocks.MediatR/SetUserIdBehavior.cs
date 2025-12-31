using BuildingBlocks.Domain;
using MediatR;

namespace BuildingBlocks.MediatR;

public class SetUserIdBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IAuditableAction
{
    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        request.CreatedByUserId = 1;
        
        return next(cancellationToken);
    }
}