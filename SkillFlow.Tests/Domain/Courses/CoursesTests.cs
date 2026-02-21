using FluentAssertions;
using SkillFlow.Domain.Courses;
using SkillFlow.Domain.Entities.Courses;
using SkillFlow.Domain.Enums;
using Xunit;

namespace SkillFlow.Tests.Domain.Courses
{
    public class CoursesTests
    {
        [Fact]
        public void Create_ShouldSetProperties_AndLeaveUpdatedAtNull()
        {
            var code = CourseCode.Create("Math", CourseType.BAS, 10);
            var name = CourseName.Create("Backend 101");
            var description = CourseDescription.Create("Intro to backend");

            var course = Course.Create(code, name, description);

            course.CourseCode.Should().Be(code);
            course.CourseType.Should().Be(code.CourseType);
            course.CourseName.Should().Be(name);
            course.CourseDescription.Should().Be(description);
            course.UpdatedAt.Should().BeNull();
        }

        [Fact]
        public void Create_WithDefaultCourseCode_ShouldThrow()
        {
            var name = CourseName.Create("Backend 101");
            var description = CourseDescription.Create("Intro to backend");

            var act = () => Course.Create(default, name, description);

            act.Should().Throw<ArgumentException>()
               .WithMessage("Coursecode is required*")
               .And.ParamName.Should().Be("code");
        }

        [Fact]
        public void UpdateCourseName_WhenDifferent_ShouldUpdate_AndSetUpdatedAt()
        {
            var course = CreateCourse();

            var newName = CourseName.Create("New name");

            course.UpdateCourseName(newName);

            course.CourseName.Should().Be(newName);
            course.UpdatedAt.Should().NotBeNull();
            course.UpdatedAt!.Value.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        }

        [Fact]
        public void UpdateCourseName_WhenSame_ShouldNotChangeUpdatedAt()
        {
            var course = CreateCourse();

            course.UpdateCourseName(CourseName.Create("Changed once"));
            var t1 = course.UpdatedAt;

            course.UpdateCourseName(CourseName.Create("Changed once"));

            course.UpdatedAt.Should().Be(t1);
        }

        [Fact]
        public void UpdateCourseDescription_WhenDifferent_ShouldUpdate_AndSetUpdatedAt()
        {
            var course = CreateCourse();

            var newDesc = CourseDescription.Create("New description");

            course.UpdateCourseDescription(newDesc);

            course.CourseDescription.Should().Be(newDesc);
            course.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void UpdateCourseDescription_WhenSame_ShouldNotChangeUpdatedAt()
        {
            var course = CreateCourse();

            course.UpdateCourseDescription(CourseDescription.Create("Changed once"));
            var t1 = course.UpdatedAt;

            course.UpdateCourseDescription(CourseDescription.Create("Changed once"));

            course.UpdatedAt.Should().Be(t1);
        }

        private static Course CreateCourse()
            => Course.Create(
                CourseCode.Create("Math", CourseType.BAS, 10),
                CourseName.Create("Backend 101"),
                CourseDescription.Create("Intro to backend")
            );
    }
}
