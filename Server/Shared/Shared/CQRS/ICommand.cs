using MediatR;
using Microsoft.Identity.Client;

namespace Shared.CQRS;

public interface ICommand : ICommand<Unit>{}

public interface ICommand<out TResponse> : IRequest<TResponse>{}

