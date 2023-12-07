using Library.Cqrs.Models.Commands;

namespace BlazorApp.Tests.Commands;

public class ErrorHandlingDecorator<TCommand, TResult> : ICommandHandler<TCommand, TResult>
    where TCommand : ICommand
{
    private readonly ICommandHandler<TCommand, TResult> _handler;

    public ErrorHandlingDecorator(ICommandHandler<TCommand, TResult> handler)
    {
        _handler = handler;
    }

    public async Task<TResult> HandleAsync(TCommand command)
    {
        try
        {
            return await _handler.HandleAsync(command);
        }
        catch (Exception ex)
        {
            // Handle the exception
            // ...
            throw;
        }
    }
}
