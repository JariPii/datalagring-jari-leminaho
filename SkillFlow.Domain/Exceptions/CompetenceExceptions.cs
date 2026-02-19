using SkillFlow.Domain.Entities.Competences;

namespace SkillFlow.Domain.Exceptions
{
    public class CompetenceNotFoundException : DomainException
    {
        public CompetenceNotFoundException(CompetenceId id) : base($"{id} could not be found") { }
        public CompetenceNotFoundException(CompetenceName name) : base($"{name.Value} could not be found") { }
    }

    public class CompetenceNameAllreadyExistsException(CompetenceName name) : DomainException($"{name} allready exists");

    public class InvalidCompetenceNameException(string message) : DomainException(message);
}

