namespace SMAIAXBackend.Domain.Repositories.Transactions;

public interface ITransactionManager
{
    Task ReadCommittedTransactionScope(Func<Task> transactionalFunction);
}