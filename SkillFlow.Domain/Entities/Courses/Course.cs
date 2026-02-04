using SkillFlow.Domain.Courses;
using SkillFlow.Domain.Interfaces;
using SkillFlow.Domain.Primitives;

namespace SkillFlow.Domain.Entities.Courses
{
    public class Course : BaseEntity<CourseId>, IAggregateRoot
    {
        public CourseName CourseName { get; private set; }
        public CourseDescription CourseDescription { get; private set; }

        public static Course Create(CourseName name, CourseDescription description)
        {
            return new Course(CourseId.New(), name, description);
        }

        protected Course(CourseId id,  CourseName name, CourseDescription description)
        {

            Id = id;
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

