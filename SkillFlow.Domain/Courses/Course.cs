using SkillFlow.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Courses
{
    public class Course : BaseEntity
    {

        public Course(CourseId id, CourseCode courseCode, string name, string description)
        {
            Id = id;
            CourseCode = courseCode;
            CourseName = name;
            CourseDescription = description;
        }

        private Course() { }

        public CourseId Id { get; private set; } = null!;
        public CourseCode CourseCode { get; private set; }
        public string CourseName { get; private set; } = null!;
        public string CourseDescription { get; private set; } = null!;

        public void UpdateCourseName(string newCourseName)
        {
            if (string.IsNullOrWhiteSpace(newCourseName))
                throw new ArgumentException("Coursename is required");

            if (CourseName == newCourseName) return;

            CourseName = newCourseName;
            UpdateTimeStamp();
        }

        public void UpdateCourseDescription(string newCourseDescription)
        {
            if (string.IsNullOrWhiteSpace(newCourseDescription))
                throw new ArgumentException("New course description is required");

            if (CourseDescription == newCourseDescription) return;

            CourseDescription = newCourseDescription;
            UpdateTimeStamp();
        }
    }

}

