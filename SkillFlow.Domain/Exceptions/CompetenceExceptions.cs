
using SkillFlow.Domain.Courses;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Exceptions
{
    public class CompetenceNotFoundException(CompetenceId id) : DomainException($"{id} could not be found");

    public class CompetenceNameAllreadyExistsException(CompetenceName name) : DomainException($"{name} allready exists");

    public class InvalidCompetenceNameException(string message) : DomainException(message);
}

