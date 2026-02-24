namespace SkillFlow.Domain.Exceptions
{
    public sealed class PersistenceException : Exception
    {
        public PersistenceException(Exception inner) : base("Persistence error", inner) { }
    }
}
