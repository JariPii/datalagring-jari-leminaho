using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Interfaces
{
    public interface ITransaction : IAsyncDisposable
    {
        Task CommitAsync(CancellationToken ct = default);
        Task RollbackAsync(CancellationToken ct = default);
    }
}
