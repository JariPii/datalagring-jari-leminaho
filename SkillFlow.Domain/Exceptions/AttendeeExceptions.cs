using SkillFlow.Domain.Attendees;
using SkillFlow.Domain.Courses;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Exceptions
{
    public class AttendeeNotFoundException(AttendeeId id) : DomainException($"Attendee {id.Value} not found");

    public class EmailNotFoundException(Email email) : DomainException($"Email {email.Value} is not registered");

    public class EmailAlreadyExistsException(Email email) : DomainException($"Email {email.Value} is already in use");

    public class InvalidRoleException(AttendeeId id, Role expectedRole) : DomainException($"Attendee {id.Value} does not have the required role: {expectedRole}");

    public class InvalidPhoneNumberException(string message) : DomainException(message);

    public class InvalidNameException(string message) : DomainException($"Invalid naming format: {message}");

    public class InvalidEmailException(string message) : DomainException(message);

    public class InstructorHasActiveSessionsException(AttendeeId id) : DomainException($"{id.Value} has active course sessions and can not be deleted");

    public class StudentHasActiveEnrollmentsException(AttendeeId id): DomainException($"{id.Value} has active enrollments and can not be deletede");

    public class InstructorIsMissingCompetenceException(AttendeeId id) : DomainException($"{id.Value} does not meet the competence requirements");

    public class CompetenceAllreadyAssignedException(AttendeeId id, CompetenceName competence) : DomainException($"{id.Value} is allready assigned {competence.Value}");


}
