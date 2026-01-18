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
            if (id.Value == Guid.Empty)
                throw new ArgumentException("Course Id can not be empty", nameof(id));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Course name is required", nameof(name));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description is required", nameof(description));

            Id = id;
            CourseCode = courseCode;
            CourseName = name.NormalizeName();
            CourseDescription = description;
        }

        private Course() { }

        public CourseId Id { get; private set; }
        public CourseCode CourseCode { get; private set; }
        public string CourseName { get; private set; } = null!;
        public string CourseDescription { get; private set; } = null!;

        public void UpdateCourseName(string newCourseName)
        {
            if (string.IsNullOrWhiteSpace(newCourseName))
                throw new ArgumentException("Coursename is required");

            var normalizedName = newCourseName.NormalizeName();

            if (CourseName == normalizedName) return;

            CourseName = normalizedName;
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

