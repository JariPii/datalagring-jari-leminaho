namespace SkillFlow.Domain.Exceptions
{
    public class MissingRowVersionException : DomainException
    {
        public MissingRowVersionException() : base("RowVersion is required") { }
    }
}
