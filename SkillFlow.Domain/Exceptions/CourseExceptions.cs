using SkillFlow.Domain.Courses;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Exceptions
{
    public class CourseNotFoundException(CourseId id) : DomainException($"Course {id.Value} could not be found");

    public class CourseCodeAllreadyExistsException(CourseCode code) : DomainException($"Course {code.Value} allready exists");

    public class CourseInUseException(CourseCode code) : DomainException($"Course {code.Value} can not be deleted because it has active sessions");

    public class CourseNameAllreadyExistsException(CourseName name) : DomainException($"Course {name.Value} allready exists");

    public class InvalidCourseDescriptionException() : DomainException($"Invalid course description");

    public class InvalidCourseNameException(string message) : DomainException(message);
}
