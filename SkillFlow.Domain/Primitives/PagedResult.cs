namespace SkillFlow.Domain.Primitives
{
    public sealed record PagedResult<T>(IEnumerable<T> Items, int Page, int PageSize, int Total);
}
