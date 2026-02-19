using Microsoft.EntityFrameworkCore.Storage;
using SkillFlow.Domain.Interfaces;

namespace SkillFlow.Infrastructure.Persistence
{
    public sealed class EfTransaction : ITransaction
    {
        private readonly IDbContextTransaction _transaction;

        public EfTransaction(IDbContextTransaction transaction)
        {
            _transaction = transaction;
        }

        public Task CommitAsync(CancellationToken ct = default) =>
            _transaction.CommitAsync(ct);

        public ValueTask DisposeAsync() =>
            _transaction.DisposeAsync();

        public Task RollbackAsync(CancellationToken ct = default) =>
            _transaction.RollbackAsync(ct);
    }
}
