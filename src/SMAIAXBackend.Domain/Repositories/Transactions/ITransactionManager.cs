namespace SMAIAXBackend.Domain.Repositories.Transactions;

public interface ITransactionManager
{
    Task TransactionScope(Func<Task> transactionalFunction);
}