using EventSourcingPoc.API.Contracts;

namespace EventSourcingPoc.API.Handlers
{
    public interface ICommandDispatcher
    {
        Task<Result> Dispatch<TCommand>(TCommand command, CancellationToken cancellationToken) where TCommand : ICommand;
        Task<Result<TResponse>> Dispatch<TCommand, TResponse>(TCommand command, CancellationToken cancellationToken) where TCommand : ICommand<TResponse>;
    }

    public interface IQueryDispatcher
    {
        Task<Result<TResponse>> Dispatch<TQuery, TResponse>(TQuery query, CancellationToken cancellationToken) where TQuery : IQuery<TResponse>;
    }

    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task<Result> Dispatch<TCommand>(TCommand command, CancellationToken cancellationToken) where TCommand : ICommand
        {
            var handler = _serviceProvider.GetService<ICommandHandler<TCommand>>() ?? throw new InvalidOperationException($"No handler registered for command type {typeof(TCommand).Name}");
            return handler.Handle(command, cancellationToken);
        }

        public Task<Result<TResponse>> Dispatch<TCommand, TResponse>(TCommand command, CancellationToken cancellationToken) where TCommand : ICommand<TResponse>
        {
            var handler = _serviceProvider.GetService<ICommandHandler<TCommand, TResponse>>() ?? throw new InvalidOperationException($"No handler registered for command type {typeof(TCommand).Name}");
            return handler.Handle(command, cancellationToken);
        }
    }

    public class QueryDispatcher : IQueryDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public QueryDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task<Result<TResponse>> Dispatch<TQuery, TResponse>(TQuery query, CancellationToken cancellationToken) where TQuery : IQuery<TResponse>
        {
            var handler = _serviceProvider.GetService<IQueryHandler<TQuery, TResponse>>() ?? throw new InvalidOperationException($"No handler registered for query type {typeof(TQuery).Name}");
            return handler.Handle(query, cancellationToken);
        }
    }
}