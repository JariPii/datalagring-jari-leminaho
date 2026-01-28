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

            if (await repository.ExistsByCourseName(courseName))
                throw new CourseNameAllreadyExistsException(courseName);

            int suffix = 1;

            var code = CourseCode.Create(dto.CityPart, dto.CourseName, dto.Year, suffix);

            while (await repository.ExistsByCourseCodeAsync(code))
            {
                suffix++;
                code = CourseCode.Create(dto.CityPart, dto.CourseName, dto.Year, suffix);
            }

            var course = new Course(
                CourseId.New(),
                code,
                courseName,
                courseDescription
                );

            await repository.AddAsync(course);

            return MatToDTO(course);
            //return new CourseDTO()
            //{
            //    Id = course.Id.Value,
            //    CourseCode = course.CourseCode.Value,
            //    CourseName = course.CourseName.Value,
            //    CourseDescription = course.CourseDescription.Value
            //};
        }

        public async Task DeleteCourseAsync(Guid id, CancellationToken ct)
        {
            var courseId = new CourseId(id);

            var success = await repository.DeleteAsync(courseId);

            if (!success)
                throw new CourseNotFoundException(courseId);
        }

        public async Task<IEnumerable<CourseDTO>> GetAllCoursesAsync(CancellationToken ct)
        {
            var courses = await repository.GetAllAsync();

            return [.. courses.Select(MatToDTO)];
            //return courses.Select(c => new CourseDTO
            //{
            //    Id = c.Id.Value,
            //    CourseCode = c.CourseCode.Value,
            //    CourseName = c.CourseName.Value,
            //    CourseDescription = c.CourseDescription.Value
            //});
        }

        public async Task<CourseDTO> GetByCourseCodeAsync(string code, CancellationToken ct)
        {
            var parsedCode = CourseCode.FromValue(code);

            var course = await repository.GetByCourseCodeAsync(parsedCode) ??
                throw new CourseNotFoundException(parsedCode);

            return MatToDTO(course);
            //return new CourseDTO
            //{
            //    Id = course.Id.Value,
            //    CourseCode = course.CourseCode.Value,
            //    CourseName = course.CourseName.Value,
            //    CourseDescription = course.CourseDescription.Value
            //};
        }

        public async Task<CourseDTO> GetCourseByIdAsync(Guid id, CancellationToken ct)
        {
            var courseId = new CourseId(id);

            var course = await repository.GetByIdAsync(courseId) ??
                throw new CourseNotFoundException(courseId);

            return MatToDTO(course);
            //return new CourseDTO
            //{
            //    Id = course.Id.Value,
            //    CourseCode = course.CourseCode.Value,
            //    CourseName = course.CourseName.Value,
            //    CourseDescription = course.CourseDescription.Value
            //};
        }

        public async Task<CourseDTO> GetCourseByNameAsync(string name, CancellationToken ct)
        {
            var courseName = CourseName.Create(name);

            var course = await repository.GetByCourseNameAsync(courseName) ??
                throw new CourseNotFoundException(courseName);

            return MatToDTO(course);
            //return new CourseDTO
            //{
            //    Id = course.Id.Value,
            //    CourseCode = course.CourseCode.Value,
            //    CourseName = course.CourseName.Value,
            //    CourseDescription = course.CourseDescription.Value
            //};

        }

        public async Task<IEnumerable<CourseDTO>> SearchCoursesAsync(string searchTerm, CancellationToken ct)
        {
            var courses = await repository.SearchByNameAsync(searchTerm);

            return [.. courses.Select(MatToDTO)];
            //return courses.Select(c => new CourseDTO
            //{
            //    Id = c.Id.Value,
            //    CourseCode = c.CourseCode.Value,
            //    CourseName = c.CourseName.Value,
            //    CourseDescription = c.CourseDescription.Value
            //});
        }

        public async Task UpdateCourseAsync(UpdateCourseDTO dto, CancellationToken ct)
        {
            var courseId = new CourseId(dto.Id);

            var course = await repository.GetByIdAsync(courseId) ??
                throw new CourseNotFoundException(courseId);

            var newName = CourseName.Create(dto.CourseName ?? course.CourseName.Value);
            var newDescription = CourseDescription.Create(dto.CourseDescription ?? course.CourseDescription.Value);

            if(course.CourseName.Value != newName.Value)
            {
                if (await repository.GetByCourseNameAsync(newName) != null)
                    throw new CourseNameAllreadyExistsException(newName);
            }

            course.UpdateCourseName(newName);
            course.UpdateCourseDescription(newDescription);

            await repository.UpdateAsync(course);
        }

        private static CourseDTO MatToDTO(Course course)
        {
            return new CourseDTO
            {
                Id = course.Id.Value,
                CourseCode = course.CourseCode.Value,
                CourseName = course.CourseName.Value,
                CourseDescription = course.CourseDescription.Value
            };
        }
    }
}
