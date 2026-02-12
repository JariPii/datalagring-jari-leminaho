using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken ct = default);
        Task<ITransaction> BeginTransactionAsync(CancellationToken ct = default);
    }
}
