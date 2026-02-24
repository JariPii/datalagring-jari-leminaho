using SkillFlow.Domain.Entities.Attendees;
using SkillFlow.Domain.Entities.Competences;
using SkillFlow.Domain.Enums;

namespace SkillFlow.Domain.Exceptions
{
    public class AttendeeNotFoundException : DomainException
    {
        public AttendeeNotFoundException(AttendeeId id) : base($"Attendee {id.Value} not found") { }
        public AttendeeNotFoundException(Email email) : base($"Attendee {email.Value} not found") { }

    }

    public class AttendeeIsRequiredException() : DomainException("An attendee is required");

    public class StudentIsRequiredException() : DomainException("A student is required");

    public class EmailNotFoundException(Email email) : DomainException($"Email {email.Value} is not registered");

    public class EmailAlreadyExistsException(Email email) : DomainException($"Email {email.Value} is already in use");

    public class InvalidRoleException : DomainException
    {
        public InvalidRoleException(AttendeeId id, Role expectedRole) : base($"Attendee {id.Value} does not have the required role: {expectedRole}") { }
        public InvalidRoleException(Role role) : base($"{role} is not a valid role, use Student or Instructor") { }
        public InvalidRoleException(string inputValue) : base($"{inputValue} is not a valid role, use Student or Instructor") { }

    }

    public class InvalidPhoneNumberException(string message) : DomainException(message);

    public class InvalidNameException(string message) : DomainException($"Invalid naming format: {message}");

    public class InvalidEmailException(string message) : DomainException(message);

    public class InstructorHasActiveSessionsException(AttendeeId id) : DomainException($"{id.Value} has active course sessions and can not be deleted");

    public class StudentHasActiveEnrollmentsException(AttendeeId id): DomainException($"{id.Value} has active enrollments and can not be deletede");

    public class InstructorIsMissingCompetenceException(AttendeeId id) : DomainException($"{id.Value} does not meet the competence requirements");

    public class CompetenceAllreadyAssignedException(AttendeeId id, CompetenceName competence) : DomainException($"{id.Value} is allready assigned {competence.Value}");
    public class InstructorNotFoundException : DomainException
    {
        public InstructorNotFoundException(AttendeeId id)
            : base($"Instructor '{id.Value}' not found") { }

        public InstructorNotFoundException(IEnumerable<AttendeeId> ids)
            : base($"Instructor(s) not found: {string.Join(", ", ids.Select(x => x.Value))}") { }
    }


}
