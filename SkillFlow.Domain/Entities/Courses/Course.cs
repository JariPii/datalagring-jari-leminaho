using SkillFlow.Domain.Courses;
using SkillFlow.Domain.Interfaces;
using SkillFlow.Domain.Primitives;

namespace SkillFlow.Domain.Entities.Courses
{
    public class Course : BaseEntity<CourseId>, IAggregateRoot
    {
        //public CourseId Id { get; private set; }
        public CourseCode CourseCode { get; private set; }
        public CourseName CourseName { get; private set; }
        public CourseDescription CourseDescription { get; private set; }

        public Course(CourseId id, CourseCode courseCode, CourseName name, CourseDescription description)
        {

            Id = id;
            CourseCode = courseCode;
            CourseName = name;
            CourseDescription = description;
        }

        private Course() { }

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

