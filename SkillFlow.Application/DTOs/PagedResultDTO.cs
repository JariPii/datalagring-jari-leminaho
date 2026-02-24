using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Application.DTOs
{
    public sealed record PagedResultDTO<T>
    {
        public required IReadOnlyList<T> Items { get; init; }
        public required int Page { get; init; }
        public required int PageSize { get; init; }
        public required int Total { get; init; }
        public int TotalPages => Total == 0
            ? 0 : (int)Math.Ceiling(Total / (double)PageSize);
        public bool HasPreviousPage => Page > 1;
        public bool HasNextPage => Page < TotalPages;
    }
}
