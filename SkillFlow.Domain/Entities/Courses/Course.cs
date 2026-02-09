using SkillFlow.Domain.Courses;
using SkillFlow.Domain.Enums;
using SkillFlow.Domain.Interfaces;
using SkillFlow.Domain.Primitives;

namespace SkillFlow.Domain.Entities.Courses
{
    public class Course : BaseEntity<CourseId>, IAggregateRoot
    {
        public CourseName CourseName { get; private set; }
        public CourseDescription CourseDescription { get; private set; }
        public CourseCode CourseCode { get; private init; }
        public CourseType CourseType { get; private init; }

        public static Course Create(CourseCode code, CourseName name, CourseDescription description)
        {
            return new Course(CourseId.New(), code, name, description);
        }

        protected Course(CourseId id, CourseCode code, CourseName name, CourseDescription description)
        {
            if (code.Equals(default))
                throw new ArgumentException("Coursecode is required", nameof(code));

            Id = id;
            CourseCode = code;
            CourseType = code.CourseType;
            CourseName = name;
            CourseDescription = description;
        }

        protected Course() { }

        public void UpdateCourseName(CourseName newCourseName)
        {
            
            if (CourseName == newCourseName) return;

            CourseName = newCourseName;
            UpdateTimeStamp();
        }

        public void UpdateCourseDescription(CourseDescription newCourseDescription)
        {

            if (CourseDescription == newCourseDescription) return;

            CourseDescription = newCourseDescription;
            UpdateTimeStamp();
        }
    }

}

