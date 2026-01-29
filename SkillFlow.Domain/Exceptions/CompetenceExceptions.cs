
using SkillFlow.Domain.Courses;
using SkillFlow.Domain.Entities.Courses;

namespace SkillFlow.Domain.Exceptions
{
    public class CompetenceNotFoundException(CompetenceId id) : DomainException($"{id} could not be found");

    public class CompetenceNameAllreadyExistsException(CompetenceName name) : DomainException($"{name} allready exists");

    public class InvalidCompetenceNameException(string message) : DomainException(message);
}

