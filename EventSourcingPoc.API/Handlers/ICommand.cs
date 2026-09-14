namespace EventSourcingPoc.API.Handlers
{
    public interface ICommand;
    public interface ICommand<TResponse>;
    public interface IQuery<TResponse>;
}