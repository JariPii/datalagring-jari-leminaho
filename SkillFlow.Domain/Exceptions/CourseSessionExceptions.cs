using SkillFlow.Domain.Attendees;
using SkillFlow.Domain.CourseSessions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Exceptions
{
    public class CourseSessionNotFoundException(CourseSessionId id) : DomainException($"{id.Value} not found");

    public class CourseSessionFullException(int capacity) : DomainException($"Course session allready has {capacity} enrollments");

    public class StudentAllreadyEnrolledException(AttendeeId studentId, CourseSessionId sessionId) : DomainException($"{studentId.Value} is allready registered to {sessionId.Value}");

    public class StudentNotEnrolledException(AttendeeId studentId, CourseSessionId sessionId) : DomainException($"{studentId.Value} is not enrollet to {sessionId.Value} yet");

    public class InstructorMissingInSessionException(CourseSessionId id) : DomainException($"{id.Value} is missing an instructor, at least one is needed");

    public class InstructorAlreadyExistsException(AttendeeId instructorId, CourseSessionId sessionId) : DomainException($"Instructor {instructorId.Value} is allready assigned to {sessionId.Value}");

    public class InvalidSessionDatesException(DateTime startDate, DateTime endDate) : DomainException($"{endDate} can not be before {startDate}");

    public class InvalidCapacityException(string message) : DomainException(message);


}
