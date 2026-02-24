using SkillFlow.Application.DTOs;
using SkillFlow.Application.DTOs.Courses;
using SkillFlow.Application.Helpers;
using SkillFlow.Application.Interfaces;
using SkillFlow.Domain.Courses;
using SkillFlow.Domain.Entities.Courses;
using SkillFlow.Domain.Exceptions;
using SkillFlow.Domain.Interfaces;

namespace SkillFlow.Application.Services.Courses
{
    public class CourseService(ICourseRepository repository, IUnitOfWork unitOfWork) : ICourseService
    {
        public async Task<CourseDTO> CreateCourseAsync(CreateCourseDTO dto, CancellationToken ct)
        {
            var courseName = CourseName.Create(dto.CourseName);
            var courseDescription = CourseDescription.Create(dto.CourseDescription);

            var prefix = CourseCode.Create(dto.CourseName, dto.CourseType).CoursePart;

            var maxSuffix = await repository.GetMaxSuffixAsync(prefix, dto.CourseType, ct);

            var nextSuffix = maxSuffix == 0 ? 10 : maxSuffix + 10;

            var code = CourseCode.Create(dto.CourseName, dto.CourseType, nextSuffix);

            var course = Course.Create(code, courseName, courseDescription);

            await repository.AddAsync(course, ct);

            await unitOfWork.SaveChangesAsync(ct);

            return MapToDTO(course);
        }

        public async Task DeleteCourseAsync(Guid id, CancellationToken ct)
        {
            var courseId = new CourseId(id);

            var course = await repository.GetByIdAsync(courseId, ct) ??
                throw new CourseNotFoundException(courseId);

            if (await repository.IsCourseInUseAsync(courseId, ct))
                throw new CourseInUseException(course.CourseName);

            repository.Remove(course);

            await unitOfWork.SaveChangesAsync(ct);
        }

        public async Task<IEnumerable<CourseDTO>> GetAllCoursesAsync(CancellationToken ct)
        {
            var courses = await repository.GetAllAsync(ct);

            return [.. courses.Select(MapToDTO)];
        }

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

        public async Task<CourseDTO> UpdateCourseAsync(Guid id, UpdateCourseDTO dto, CancellationToken ct)
        {
            var courseId = new CourseId(id);

            var course = await repository.GetByIdAsync(courseId, ct) ??
                throw new CourseNotFoundException(courseId);

            var newName = CourseName.Create(dto.CourseName ?? course.CourseName.Value);
            var newDescription = CourseDescription.Create(dto.CourseDescription ?? course.CourseDescription.Value);

            if(course.CourseName != newName && await repository.ExistsByCourseName(newName, ct))
                throw new CourseNameAllreadyExistsException(newName);
            

            course.UpdateCourseName(newName);
            course.UpdateCourseDescription(newDescription);

            await repository.UpdateAsync(course, dto.RowVersion, ct);

            await unitOfWork.SaveChangesAsync(ct);

            return MapToDTO(course);
        }

        public static CourseDTO MapToDTO(Course course)
        {
            var type = course.CourseCode.CourseType;
            return new CourseDTO
            {
                Id = course.Id.Value,
                CourseCode = course.CourseCode.Value,
                CourseType = type,
                CourseTypeName = EnumDisplyName.GetDisplayName(type),
                CourseName = course.CourseName.Value,
                CourseDescription = course.CourseDescription.Value,
                RowVersion = course.RowVersion
            };
        }

        public async Task<PagedResultDTO<CourseDTO>> GetCoursesPagedAsync(int page, int pageSize, string? q, CancellationToken ct = default)
        {
            var result = await repository.GetCoursePagedAsync(page, pageSize, q, ct);

            var items = result.Items.Select(MapToDTO).ToList();

            return new PagedResultDTO<CourseDTO>
            {
                Items = items,
                Page = result.Page,
                PageSize = result.PageSize,
                Total = result.Total
            };
        }
    }
}
