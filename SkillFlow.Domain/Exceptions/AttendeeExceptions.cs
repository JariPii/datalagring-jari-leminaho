using SkillFlow.Domain.Courses;
using SkillFlow.Domain.Entities.Attendees;
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

    public class InstructorIsRequiredException() : DomainException("An instructor is required");

    public class EmailNotFoundException(Email email) : DomainException($"Email {email.Value} is not registered");

    public class EmailAlreadyExistsException(Email email) : DomainException($"Email {email.Value} is already in use");

    public class InvalidRoleException : DomainException
    {
        public InvalidRoleException(AttendeeId id, Role expectedRole) : base($"Attendee {id.Value} does not have the required role: {expectedRole}") { }
        public InvalidRoleException(string role) : base($"{role} is not a valid role, use Student or Instructor") { }

    }

    public class InvalidPhoneNumberException(string message) : DomainException(message);

    public class InvalidNameException(string message) : DomainException($"Invalid naming format: {message}");

    public class InvalidEmailException(string message) : DomainException(message);

    public class InstructorHasActiveSessionsException(AttendeeId id) : DomainException($"{id.Value} has active course sessions and can not be deleted");

    public class StudentHasActiveEnrollmentsException(AttendeeId id): DomainException($"{id.Value} has active enrollments and can not be deletede");

    public class InstructorIsMissingCompetenceException(AttendeeId id) : DomainException($"{id.Value} does not meet the competence requirements");

    public class CompetenceAllreadyAssignedException(AttendeeId id, CompetenceName competence) : DomainException($"{id.Value} is allready assigned {competence.Value}");


}
