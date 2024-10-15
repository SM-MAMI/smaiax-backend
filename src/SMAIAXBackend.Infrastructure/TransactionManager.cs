using System.Transactions;
using SMAIAXBackend.Domain.Repositories.Transactions;

namespace SMAIAXBackend.Infrastructure;

public class TransactionManager : ITransactionManager
{
    public async Task TransactionScope(Func<Task> transactionalFunction)
    {
        using var scope =
            new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);

        await transactionalFunction();

        // The Complete method commits the transaction. If an exception has been thrown,
        // Complete is not  called and the transaction is rolled back.
        scope.Complete();
    }
}