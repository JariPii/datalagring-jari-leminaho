using SkillFlow.Application.DTOs.Courses;
using SkillFlow.Application.Interfaces;
using SkillFlow.Domain.Courses;
using SkillFlow.Domain.Entities.Courses;
using SkillFlow.Domain.Exceptions;
using SkillFlow.Domain.Interfaces;

namespace SkillFlow.Application.Services.Courses
{
    public class CourseService(ICourseRepository repository) : ICourseService
    {
        public async Task<CourseDTO> CreateCourseAsync(CreateCourseDTO dto, CancellationToken ct)
        {
            var courseName = CourseName.Create(dto.CourseName);
            var courseDescription = CourseDescription.Create(dto.CourseDescription);

            if (await repository.ExistsByCourseName(courseName, ct))
                throw new CourseNameAllreadyExistsException(courseName);            

            var course = Course.Create(courseName, courseDescription);

            await repository.AddAsync(course, ct);

            return MapToDTO(course);
        }

        public async Task DeleteCourseAsync(Guid id, CancellationToken ct)
        {
            var courseId = new CourseId(id);

            var success = await repository.DeleteAsync(courseId, ct);

            if (!success)
                throw new CourseNotFoundException(courseId);
        }

        public async Task<IEnumerable<CourseDTO>> GetAllCoursesAsync(CancellationToken ct)
        {
            var courses = await repository.GetAllAsync(ct);

            return [.. courses.Select(MapToDTO)];
        }

        //public async Task<CourseDTO> GetByCourseCodeAsync(string code, CancellationToken ct)
        //{
        //    var parsedCode = CourseCode.FromValue(code);

        //    var course = await repository.GetByCourseCodeAsync(parsedCode, ct) ??
        //        throw new CourseNotFoundException(parsedCode);

        //    return MapToDTO(course);
        //}

        public async Task<CourseDTO> GetCourseByIdAsync(Guid id, CancellationToken ct)
        {
            var courseId = new CourseId(id);

            var course = await repository.GetByIdAsync(courseId, ct) ??
                throw new CourseNotFoundException(courseId);

            return MapToDTO(course);
        }

        public async Task<CourseDTO> GetCourseByNameAsync(string name, CancellationToken ct)
        {
            var courseName = CourseName.Create(name);

            var course = await repository.GetByCourseNameAsync(courseName, ct) ??
                throw new CourseNotFoundException(courseName);

            return MapToDTO(course);

        }

        public async Task<IEnumerable<CourseDTO>> SearchCoursesAsync(string searchTerm, CancellationToken ct)
        {
            var courses = await repository.SearchByNameAsync(searchTerm, ct);

            return [.. courses.Select(MapToDTO)];
        }

        public async Task<CourseDTO> UpdateCourseAsync(UpdateCourseDTO dto, CancellationToken ct)
        {
            var courseId = new CourseId(dto.Id);

            var course = await repository.GetByIdAsync(courseId, ct) ??
                throw new CourseNotFoundException(courseId);

            var newName = CourseName.Create(dto.CourseName ?? course.CourseName.Value);
            var newDescription = CourseDescription.Create(dto.CourseDescription ?? course.CourseDescription.Value);

            if(course.CourseName != newName && await repository.ExistsByCourseName(newName, ct))
                throw new CourseNameAllreadyExistsException(newName);
            

            course.UpdateCourseName(newName);
            course.UpdateCourseDescription(newDescription);

            await repository.UpdateAsync(course, dto.RowVersion, ct);

            return MapToDTO(course);
        }

        private static CourseDTO MapToDTO(Course course)
        {
            return new CourseDTO
            {
                Id = course.Id.Value,
                CourseName = course.CourseName.Value,
                CourseDescription = course.CourseDescription.Value,
                RowVersion = course.RowVersion
            };
        }
    }
}
