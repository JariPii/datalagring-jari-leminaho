using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using SkillFlow.Application.DTOs.Courses;
using SkillFlow.Application.Services.Courses;
using SkillFlow.Domain.Courses;
using SkillFlow.Domain.Entities.Courses;
using SkillFlow.Domain.Enums;
using SkillFlow.Domain.Exceptions;
using SkillFlow.Domain.Interfaces;
using Xunit;

namespace SkillFlow.Tests.Application.Services
{
    public class CourseServiceTests
    {
        private readonly Mock<ICourseRepository> _repo = new();
        private readonly Mock<IUnitOfWork> _uow = new();

        private CourseService CreateSut() => new(_repo.Object, _uow.Object);

        // -------------------------
        // CreateCourseAsync
        // -------------------------

        [Fact]
        public async Task CreateCourseAsync_WhenMaxSuffixIs0_ShouldUse10()
        {
            var sut = CreateSut();
            var dto = ValidCreateDto();

            _repo.Setup(x => x.GetMaxSuffixAsync(It.IsAny<string>(), dto.CourseType, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(0);

            Course? added = null;
            _repo.Setup(x => x.AddAsync(It.IsAny<Course>(), It.IsAny<CancellationToken>()))
                 .Callback<Course, CancellationToken>((c, _) => added = c)
                 .Returns(Task.CompletedTask);

            _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await sut.CreateCourseAsync(dto, CancellationToken.None);

            added.Should().NotBeNull();
            added!.CourseCode.CourseSuffix.Should().Be(10);

            _repo.Verify(x => x.AddAsync(It.IsAny<Course>(), It.IsAny<CancellationToken>()), Times.Once);
            _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            result.CourseCode.Should().Be(added.CourseCode.Value);
            result.CourseName.Should().Be(CourseName.Create(dto.CourseName).Value);
            result.CourseDescription.Should().Be(CourseDescription.Create(dto.CourseDescription).Value);
            result.CourseType.Should().Be(dto.CourseType);
        }

        [Fact]
        public async Task CreateCourseAsync_WhenMaxSuffixIs20_ShouldUse30()
        {
            var sut = CreateSut();
            var dto = ValidCreateDto();

            _repo.Setup(x => x.GetMaxSuffixAsync(It.IsAny<string>(), dto.CourseType, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(20);

            Course? added = null;
            _repo.Setup(x => x.AddAsync(It.IsAny<Course>(), It.IsAny<CancellationToken>()))
                 .Callback<Course, CancellationToken>((c, _) => added = c)
                 .Returns(Task.CompletedTask);

            _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            await sut.CreateCourseAsync(dto, CancellationToken.None);

            added.Should().NotBeNull();
            added!.CourseCode.CourseSuffix.Should().Be(30);
        }

        [Fact]
        public async Task CreateCourseAsync_ShouldCallGetMaxSuffixWithComputedPrefix()
        {
            var sut = CreateSut();
            var dto = ValidCreateDto(courseName: "Math");

            var expectedPrefix = CourseCode.Create(dto.CourseName, dto.CourseType).CoursePart;

            _repo.Setup(x => x.GetMaxSuffixAsync(expectedPrefix, dto.CourseType, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(0);

            _repo.Setup(x => x.AddAsync(It.IsAny<Course>(), It.IsAny<CancellationToken>()))
                 .Returns(Task.CompletedTask);

            _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            await sut.CreateCourseAsync(dto, CancellationToken.None);

            _repo.Verify(x => x.GetMaxSuffixAsync(expectedPrefix, dto.CourseType, It.IsAny<CancellationToken>()), Times.Once);
        }

        // -------------------------
        // DeleteCourseAsync
        // -------------------------

        [Fact]
        public async Task DeleteCourseAsync_WhenNotFound_ShouldThrow()
        {
            var sut = CreateSut();

            _repo.Setup(x => x.GetByIdAsync(It.IsAny<CourseId>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync((Course?)null);

            var act = async () => await sut.DeleteCourseAsync(Guid.NewGuid(), CancellationToken.None);

            await act.Should().ThrowAsync<CourseNotFoundException>();

            _repo.Verify(x => x.Remove(It.IsAny<Course>()), Times.Never);
            _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteCourseAsync_HappyPath_ShouldRemove_AndSave()
        {
            var sut = CreateSut();

            var course = CreateCourse();

            _repo.Setup(x => x.GetByIdAsync(It.IsAny<CourseId>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(course);

            _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            await sut.DeleteCourseAsync(Guid.NewGuid(), CancellationToken.None);

            _repo.Verify(x => x.Remove(course), Times.Once);
            _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteCourseAsync_WhenDbUpdateException_ShouldThrowCourseInUseException()
        {
            var sut = CreateSut();

            var course = CreateCourse();

            _repo.Setup(x => x.GetByIdAsync(It.IsAny<CourseId>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(course);

            _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new DbUpdateException("FK"));

            var act = async () => await sut.DeleteCourseAsync(Guid.NewGuid(), CancellationToken.None);

            await act.Should().ThrowAsync<CourseInUseException>();

            _repo.Verify(x => x.Remove(course), Times.Once);
        }

        // -------------------------
        // GetAllCoursesAsync
        // -------------------------

        [Fact]
        public async Task GetAllCoursesAsync_ShouldReturnMappedDtos()
        {
            var sut = CreateSut();

            var courses = new List<Course>
            {
                CreateCourse(courseName: "Backend"),
                CreateCourse(courseName: "Frontend")
            };

            _repo.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                 .ReturnsAsync(courses);

            var result = (await sut.GetAllCoursesAsync(CancellationToken.None)).ToList();

            result.Should().HaveCount(2);
            result.Select(r => r.CourseName).Should().Contain(new[] { "Backend", "Frontend" });
        }

        // -------------------------
        // GetCourseByIdAsync
        // -------------------------

        [Fact]
        public async Task GetCourseByIdAsync_WhenNotFound_ShouldThrow()
        {
            var sut = CreateSut();

            _repo.Setup(x => x.GetByIdAsync(It.IsAny<CourseId>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync((Course?)null);

            var act = async () => await sut.GetCourseByIdAsync(Guid.NewGuid(), CancellationToken.None);

            await act.Should().ThrowAsync<CourseNotFoundException>();
        }

        [Fact]
        public async Task GetCourseByIdAsync_WhenFound_ShouldReturnDto()
        {
            var sut = CreateSut();

            var course = CreateCourse(courseName: "Backend");

            _repo.Setup(x => x.GetByIdAsync(It.IsAny<CourseId>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(course);

            var result = await sut.GetCourseByIdAsync(Guid.NewGuid(), CancellationToken.None);

            result.CourseName.Should().Be("Backend");
            result.CourseCode.Should().Be(course.CourseCode.Value);
        }

        // -------------------------
        // GetCourseByNameAsync
        // -------------------------

        [Fact]
        public async Task GetCourseByNameAsync_WhenNotFound_ShouldThrow()
        {
            var sut = CreateSut();

            _repo.Setup(x => x.GetByCourseNameAsync(It.IsAny<CourseName>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync((Course?)null);

            var act = async () => await sut.GetCourseByNameAsync("Backend", CancellationToken.None);

            await act.Should().ThrowAsync<CourseNotFoundException>();
        }

        [Fact]
        public async Task GetCourseByNameAsync_WhenFound_ShouldReturnDto()
        {
            var sut = CreateSut();

            var course = CreateCourse(courseName: "Backend");

            _repo.Setup(x => x.GetByCourseNameAsync(It.IsAny<CourseName>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(course);

            var result = await sut.GetCourseByNameAsync("backend", CancellationToken.None);

            // NormalizeName => "Backend"
            result.CourseName.Should().Be("Backend");
        }

        // -------------------------
        // SearchCoursesAsync
        // -------------------------

        [Fact]
        public async Task SearchCoursesAsync_ShouldReturnMappedDtos()
        {
            var sut = CreateSut();

            var courses = new List<Course>
            {
                CreateCourse(courseName: "Backend"),
                CreateCourse(courseName: "Backend Advanced"),
            };

            _repo.Setup(x => x.SearchByNameAsync("back", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(courses);

            var result = (await sut.SearchCoursesAsync("back", CancellationToken.None)).ToList();

            result.Should().HaveCount(2);
            result.All(x => x.CourseName.StartsWith("Backend")).Should().BeTrue();
        }

        // -------------------------
        // UpdateCourseAsync
        // -------------------------

        [Fact]
        public async Task UpdateCourseAsync_WhenNotFound_ShouldThrow()
        {
            var sut = CreateSut();

            _repo.Setup(x => x.GetByIdAsync(It.IsAny<CourseId>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync((Course?)null);

            var dto = new UpdateCourseDTO
            {
                CourseName = "New Name",
                CourseDescription = "New Desc",
                RowVersion = new byte[] { 1 }
            };

            var act = async () => await sut.UpdateCourseAsync(Guid.NewGuid(), dto, CancellationToken.None);

            await act.Should().ThrowAsync<CourseNotFoundException>();

            _repo.Verify(x => x.UpdateAsync(It.IsAny<Course>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()), Times.Never);
            _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdateCourseAsync_WhenNameChangedAndExists_ShouldThrow()
        {
            var sut = CreateSut();

            var course = CreateCourse(courseName: "Backend");

            _repo.Setup(x => x.GetByIdAsync(It.IsAny<CourseId>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(course);

            _repo.Setup(x => x.ExistsByCourseName(It.IsAny<CourseName>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(true);

            var dto = new UpdateCourseDTO
            {
                CourseName = "Another Name",
                CourseDescription = course.CourseDescription.Value,
                RowVersion = new byte[] { 1 }
            };

            var act = async () => await sut.UpdateCourseAsync(Guid.NewGuid(), dto, CancellationToken.None);

            await act.Should().ThrowAsync<CourseNameAllreadyExistsException>();

            _repo.Verify(x => x.UpdateAsync(It.IsAny<Course>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()), Times.Never);
            _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdateCourseAsync_WhenNameIsSameAfterNormalization_ShouldNotCheckExists()
        {
            var sut = CreateSut();

            var course = CreateCourse(courseName: "Backend Development");

            _repo.Setup(x => x.GetByIdAsync(It.IsAny<CourseId>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(course);

            _repo.Setup(x => x.UpdateAsync(It.IsAny<Course>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()))
                 .Returns(Task.CompletedTask);

            _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var dto = new UpdateCourseDTO
            {
                // normaliseras till "Backend Development" => samma
                CourseName = "  backend   development ",
                CourseDescription = null, // behåll befintlig
                RowVersion = new byte[] { 7 }
            };

            var result = await sut.UpdateCourseAsync(Guid.NewGuid(), dto, CancellationToken.None);

            _repo.Verify(x => x.ExistsByCourseName(It.IsAny<CourseName>(), It.IsAny<CancellationToken>()), Times.Never);
            _repo.Verify(x => x.UpdateAsync(course, dto.RowVersion, It.IsAny<CancellationToken>()), Times.Once);
            _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            result.CourseName.Should().Be("Backend Development");
            result.CourseDescription.Should().Be(course.CourseDescription.Value);
        }

        [Fact]
        public async Task UpdateCourseAsync_HappyPath_ShouldUpdate_Save_AndReturnDto()
        {
            var sut = CreateSut();

            var course = CreateCourse(courseName: "Backend", description: "Old");

            _repo.Setup(x => x.GetByIdAsync(It.IsAny<CourseId>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(course);

            _repo.Setup(x => x.ExistsByCourseName(It.IsAny<CourseName>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(false);

            _repo.Setup(x => x.UpdateAsync(It.IsAny<Course>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()))
                 .Returns(Task.CompletedTask);

            _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var dto = new UpdateCourseDTO
            {
                CourseName = "New Name",
                CourseDescription = "New Desc",
                RowVersion = new byte[] { 1, 2 }
            };

            var result = await sut.UpdateCourseAsync(Guid.NewGuid(), dto, CancellationToken.None);

            course.CourseName.Should().Be(CourseName.Create("New Name"));
            course.CourseDescription.Should().Be(CourseDescription.Create("New Desc"));

            _repo.Verify(x => x.UpdateAsync(course, dto.RowVersion, It.IsAny<CancellationToken>()), Times.Once);
            _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            result.CourseName.Should().Be(course.CourseName.Value);
            result.CourseDescription.Should().Be(course.CourseDescription.Value);
        }

        // -------------------------
        // Helpers
        // -------------------------

        private static CreateCourseDTO ValidCreateDto(string courseName = "Backend", CourseType type = CourseType.BAS)
            => new()
            {
                CourseName = courseName,
                CourseDescription = "Intro",
                CourseType = type
            };

        private static Course CreateCourse(string courseName = "Backend", string description = "Desc", CourseType type = CourseType.BAS, int suffix = 10)
        {
            var code = CourseCode.Create(courseName, type, suffix);

            return Course.Create(
                code,
                CourseName.Create(courseName),
                CourseDescription.Create(description));
        }
    }
}
