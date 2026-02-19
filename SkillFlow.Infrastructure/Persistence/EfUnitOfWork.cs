using SkillFlow.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Infrastructure.Persistence
{
    public sealed class EfUnitOfWork : IUnitOfWork
    {
        private readonly SkillFlowDbContext _context;

        public EfUnitOfWork(SkillFlowDbContext context)
        {
            _context = context;
        }

        public async Task<ITransaction> BeginTransactionAsync(CancellationToken ct = default)
        {
            var transaction = await _context.Database.BeginTransactionAsync(ct);
            return new EfTransaction(transaction);
        }

        public Task<int> SaveChangesAsync(CancellationToken ct = default) =>
            _context.SaveChangesAsync(ct);
    }
}
