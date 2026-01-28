using SkillFlow.Domain.Courses;
using SkillFlow.Domain.Entities.Courses;

namespace SkillFlow.Domain.Exceptions
{
    //public class CourseNotFoundException(CourseCode code) : DomainException($"Course {code.Value} could not be found");

    public class CourseNotFoundException : DomainException
    {
        public CourseNotFoundException(CourseCode code) : base($"Course {code.Value} could not be found") { }

        public CourseNotFoundException(CourseId id) : base($"Course {id.Value} could not be found") { }

        public CourseNotFoundException(CourseName name) : base($"Course {name.Value} could not be found") { }
    }

    public class CourseCodeAllreadyExistsException(CourseCode code) : DomainException($"Course {code.Value} allready exists");

    public class CourseInUseException(CourseCode code) : DomainException($"Course {code.Value} can not be deleted because it has active sessions");

    public class CourseNameAllreadyExistsException(CourseName name) : DomainException($"Course {name.Value} allready exists");

    public class InvalidCourseDescriptionException() : DomainException($"Invalid course description");

    public class InvalidCourseNameException(string message) : DomainException(message);
}
