using System.Diagnostics.CodeAnalysis;
using LunaArch.Abstractions.Messaging;
using LunaArch.Abstractions.Persistence;

namespace LunaArch.Application.Behaviors;

/// <summary>
/// Pipeline behavior that wraps command handling in a transaction.
/// Automatically commits on success or rolls back on failure.
/// </summary>
/// <typeparam name="TRequest">The request type (should be a command).</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public sealed class UnitOfWorkBehavior<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)] TRequest, 
    TResponse>(
    IUnitOfWork unitOfWork) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    /// <inheritdoc />
    public async Task<TResponse> HandleAsync(
        TRequest request,
        PipelineDelegate<TResponse> continuation,
        CancellationToken cancellationToken = default)
    {
        // Only wrap commands in transactions, not queries
        if (!IsCommand())
        {
            return await continuation(cancellationToken);
        }

        await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var response = await continuation(cancellationToken);
            await unitOfWork.CommitTransactionAsync(cancellationToken);
            return response;
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    private static bool IsCommand()
    {
        return typeof(TRequest).GetInterfaces()
            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommand<>));
    }
}
