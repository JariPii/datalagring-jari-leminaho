using FluentAssertions;
using Moq;
using SkillFlow.Application.DTOs.Attendees;
using SkillFlow.Application.Services.Attendees;
using SkillFlow.Domain.Attendees;
using SkillFlow.Domain.Entities.Attendees;
using SkillFlow.Domain.Entities.Competences;
using SkillFlow.Domain.Enums;
using SkillFlow.Domain.Exceptions;
using SkillFlow.Domain.Interfaces;
using Xunit;

namespace SkillFlow.Tests.Application.Services
{
    public class AttendeeServiceTests
    {
        private readonly Mock<IAttendeeRepository> _repo = new();
        private readonly Mock<IUnitOfWork> _uow = new();
        private readonly Mock<IAttendeeQueries> _queries = new();
        private readonly Mock<ICompetenceRepository> _competenceRepo = new();

        private AttendeeService CreateSut()
            => new AttendeeService(_repo.Object, _uow.Object, _queries.Object, _competenceRepo.Object);

        private static CreateAttendeeDTO CreateDto(Role role, string email = "test@gmail.com")
            => new()
            {
                Email = email,
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = null,
                Role = role
            };

        private static Attendee CreateStudent(string email = "old@gmail.com")
            => Attendee.CreateStudent(
                Email.Create(email),
                AttendeeName.Create("John", "Doe"),
                PhoneNumber.Create(null));

        private static Instructor CreateInstructor(string email = "teacher@gmail.com")
            => Attendee.CreateInstructor(
                Email.Create(email),
                AttendeeName.Create("Ada", "Lovelace"),
                PhoneNumber.Create(null));

        [Fact]
        public async Task CreateAttendeeAsync_should_throw_when_email_already_exists()
        {
            var dto = CreateDto(Role.Student, "test@gmail.com");

            _repo.Setup(r => r.ExistsByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var sut = CreateSut();

            Func<Task> act = () => sut.CreateAttendeeAsync(dto, CancellationToken.None);

            await act.Should().ThrowAsync<EmailAlreadyExistsException>();

            _repo.Verify(r => r.AddAsync(It.IsAny<Attendee>(), It.IsAny<CancellationToken>()), Times.Never);
            _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData(Role.Student, typeof(StudentDTO))]
        [InlineData(Role.Instructor, typeof(InstructorDTO))]
        public async Task CreateAttendeeAsync_should_create_expected_type_and_save(Role role, Type expectedDtoType)
        {
            var dto = CreateDto(role, "teacher@gmail.com");

            _repo.Setup(r => r.ExistsByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            Attendee? added = null;
            _repo.Setup(r => r.AddAsync(It.IsAny<Attendee>(), It.IsAny<CancellationToken>()))
                .Callback<Attendee, CancellationToken>((a, _) => added = a)
                .Returns(Task.CompletedTask);

            _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var sut = CreateSut();

            var result = await sut.CreateAttendeeAsync(dto, CancellationToken.None);

            added.Should().NotBeNull();
            added!.Role.Should().Be(role);
            added.Email.Value.Should().Be("teacher@gmail.com");

            result.Should().BeOfType(expectedDtoType);
            result.Email.Should().Be("teacher@gmail.com");

            _repo.Verify(r => r.AddAsync(It.IsAny<Attendee>(), It.IsAny<CancellationToken>()), Times.Once);
            _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAttendeeAsync_should_throw_when_not_found()
        {
            var id = Guid.NewGuid();

            _repo.Setup(r => r.DeleteAsync(It.IsAny<AttendeeId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var sut = CreateSut();

            Func<Task> act = () => sut.DeleteAttendeeAsync(id, CancellationToken.None);

            await act.Should().ThrowAsync<AttendeeNotFoundException>();

            _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAttendeeAsync_should_save_when_deleted()
        {
            var id = Guid.NewGuid();

            _repo.Setup(r => r.DeleteAsync(It.IsAny<AttendeeId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var sut = CreateSut();

            await sut.DeleteAttendeeAsync(id, CancellationToken.None);

            _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAttendeeByEmailAsync_should_throw_when_not_found()
        {
            _repo.Setup(r => r.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Attendee?)null);

            var sut = CreateSut();

            Func<Task> act = () => sut.GetAttendeeByEmailAsync("test@gmail.com", CancellationToken.None);

            await act.Should().ThrowAsync<AttendeeNotFoundException>();
        }

        [Fact]
        public async Task UpdateAttendeeAsync_should_throw_when_new_email_already_exists()
        {
            var id = Guid.NewGuid();
            var existing = CreateStudent("old@gmail.com");

            _repo.Setup(r => r.GetByIdAsync(It.IsAny<AttendeeId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existing);

            _repo.Setup(r => r.ExistsByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var dto = new UpdateAttendeeDTO
            {
                Email = "new@gmail.com",
                FirstName = null,
                LastName = null,
                PhoneNumber = null,
                RowVersion = new byte[] { 1, 2, 3 }
            };

            var sut = CreateSut();

            Func<Task> act = () => sut.UpdateAttendeeAsync(id, dto, CancellationToken.None);

            await act.Should().ThrowAsync<EmailAlreadyExistsException>();

            _repo.Verify(r => r.UpdateAsync(It.IsAny<Attendee>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()), Times.Never);
            _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAttendeeAsync_should_update_and_save()
        {
            var id = Guid.NewGuid();
            var existing = CreateStudent("old@gmail.com");

            _repo.Setup(r => r.GetByIdAsync(It.IsAny<AttendeeId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existing);

            _repo.Setup(r => r.ExistsByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _repo.Setup(r => r.UpdateAsync(It.IsAny<Attendee>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var dto = new UpdateAttendeeDTO
            {
                Email = "new@gmail.com",
                FirstName = "Ada",
                LastName = "Lovelace",
                PhoneNumber = "+46701234567", // E.164
                RowVersion = new byte[] { 9, 9, 9 }
            };

            var sut = CreateSut();

            var result = await sut.UpdateAttendeeAsync(id, dto, CancellationToken.None);

            result.Email.Should().Be("new@gmail.com");
            result.FirstName.Should().Be("Ada");
            result.LastName.Should().Be("Lovelace");
            result.PhoneNumber.Should().Be("+46701234567");

            _repo.Verify(r => r.UpdateAsync(It.IsAny<Attendee>(), dto.RowVersion, It.IsAny<CancellationToken>()), Times.Once);
            _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddCompetenceToInstructorAsync_should_commit_on_success()
        {
            var instructorId = Guid.NewGuid();
            var rowVersion = new byte[] { 1, 1, 1 };

            var instructor = CreateInstructor("teacher@gmail.com");

            var tx = new Mock<ITransaction>();
            _uow.Setup(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(tx.Object);

            _repo.Setup(r => r.GetByIdAsync(It.IsAny<AttendeeId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(instructor);

            var competence = Competence.Create(CompetenceName.Create("C#"));
            _competenceRepo.Setup(c => c.GetByNameAsync(It.IsAny<CompetenceName>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(competence);

            _repo.Setup(r => r.UpdateAsync(It.IsAny<Instructor>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var sut = CreateSut();

            await sut.AddCompetenceToInstructorAsync(instructorId, "C#", rowVersion, CancellationToken.None);

            tx.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            tx.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);

            _repo.Verify(r => r.UpdateAsync(It.IsAny<Instructor>(), rowVersion, It.IsAny<CancellationToken>()), Times.Once);
            _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddCompetenceToInstructorAsync_should_rollback_when_attendee_not_found()
        {
            var instructorId = Guid.NewGuid();
            var rowVersion = new byte[] { 1, 1, 1 };

            var tx = new Mock<ITransaction>();
            _uow.Setup(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(tx.Object);

            _repo.Setup(r => r.GetByIdAsync(It.IsAny<AttendeeId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Attendee?)null);

            var sut = CreateSut();

            Func<Task> act = () =>
                sut.AddCompetenceToInstructorAsync(instructorId, "C#", rowVersion, CancellationToken.None);

            await act.Should().ThrowAsync<AttendeeNotFoundException>();

            tx.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
            tx.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);

            _repo.Verify(r => r.UpdateAsync(It.IsAny<Instructor>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()), Times.Never);
            _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAttendeeAsync_should_throw_when_attendee_not_found()
        {
            var id = Guid.NewGuid();

            _repo.Setup(r => r.GetByIdAsync(It.IsAny<AttendeeId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Attendee?)null);

            var dto = new UpdateAttendeeDTO
            {
                Email = "new@gmail.com",
                FirstName = null,
                LastName = null,
                PhoneNumber = null,
                RowVersion = new byte[] { 1, 2, 3 }
            };

            var sut = CreateSut();

            Func<Task> act = () => sut.UpdateAttendeeAsync(id, dto, CancellationToken.None);

            await act.Should().ThrowAsync<AttendeeNotFoundException>();

            _repo.Verify(r => r.UpdateAsync(It.IsAny<Attendee>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()), Times.Never);
            _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task AddCompetenceToInstructorAsync_should_throw_when_competence_not_found()
        {
            var instructorId = Guid.NewGuid();
            var rowVersion = new byte[] { 1, 1, 1 };

            var tx = new Mock<ITransaction>();
            _uow.Setup(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(tx.Object);

            var instructor = CreateInstructor("teacher@gmail.com");

            _repo.Setup(r => r.GetByIdAsync(It.IsAny<AttendeeId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(instructor);

            _competenceRepo.Setup(c => c.GetByNameAsync(It.IsAny<CompetenceName>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Competence?)null);

            var sut = CreateSut();

            Func<Task> act = () =>
                sut.AddCompetenceToInstructorAsync(instructorId, "C#", rowVersion, CancellationToken.None);

            await act.Should().ThrowAsync<CompetenceNotFoundException>();

            tx.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
            tx.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);

            _repo.Verify(r => r.UpdateAsync(It.IsAny<Instructor>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()), Times.Never);
            _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task AddCompetenceToInstructorAsync_should_throw_when_attendee_is_not_instructor()
        {
            var instructorId = Guid.NewGuid();
            var rowVersion = new byte[] { 1, 1, 1 };

            var tx = new Mock<ITransaction>();
            _uow.Setup(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(tx.Object);

            var student = CreateStudent("student@gmail.com");

            _repo.Setup(r => r.GetByIdAsync(It.IsAny<AttendeeId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(student);

            var sut = CreateSut();

            Func<Task> act = () =>
                sut.AddCompetenceToInstructorAsync(instructorId, "C#", rowVersion, CancellationToken.None);

            await act.Should().ThrowAsync<InvalidRoleException>();

            tx.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
            tx.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);

            _repo.Verify(r => r.UpdateAsync(It.IsAny<Instructor>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()), Times.Never);
            _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetAttendeeByEmailAsync_should_return_expected_type_when_found()
        {
            var attendee = CreateInstructor("teacher@gmail.com");

            _repo.Setup(r => r.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(attendee);

            var sut = CreateSut();

            var result = await sut.GetAttendeeByEmailAsync("teacher@gmail.com", CancellationToken.None);

            result.Should().BeOfType<InstructorDTO>();
            result.Email.Should().Be("teacher@gmail.com");
        }

        [Fact]
        public async Task GetAttendeeByIdAsync_should_throw_when_not_found()
        {
            _repo.Setup(r => r.GetByIdAsync(It.IsAny<AttendeeId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Attendee?)null);

            var sut = CreateSut();

            Func<Task> act = () => sut.GetAttendeeByIdAsync(Guid.NewGuid(), CancellationToken.None);

            await act.Should().ThrowAsync<AttendeeNotFoundException>();
        }

        [Fact]
        public async Task GetAttendeeByIdAsync_should_return_expected_type_when_found()
        {
            var id = Guid.NewGuid();
            var attendee = CreateStudent("student@gmail.com");

            _repo.Setup(r => r.GetByIdAsync(It.IsAny<AttendeeId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(attendee);

            var sut = CreateSut();

            var result = await sut.GetAttendeeByIdAsync(id, CancellationToken.None);

            result.Should().BeOfType<StudentDTO>();
            result.Email.Should().Be("student@gmail.com");
        }

        [Fact]
        public async Task UpdateAttendeeAsync_should_not_check_email_uniqueness_when_email_is_null()
        {
            var id = Guid.NewGuid();
            var existing = CreateStudent("old@gmail.com");

            _repo.Setup(r => r.GetByIdAsync(It.IsAny<AttendeeId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existing);

            _repo.Setup(r => r.UpdateAsync(It.IsAny<Attendee>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var dto = new UpdateAttendeeDTO
            {
                Email = null,
                FirstName = "Newname",
                LastName = null,
                PhoneNumber = null,
                RowVersion = new byte[] { 1, 2, 3 }
            };

            var sut = CreateSut();

            var result = await sut.UpdateAttendeeAsync(id, dto, CancellationToken.None);

            result.FirstName.Should().Be("Newname");

            _repo.Verify(r => r.ExistsByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()), Times.Never);
            _repo.Verify(r => r.UpdateAsync(It.IsAny<Attendee>(), dto.RowVersion, It.IsAny<CancellationToken>()), Times.Once);
            _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAllAttendeesAsync_should_call_queries_and_return_result()
        {
            var attendee = CreateStudent("a@gmail.com");

            _queries.Setup(q => q.GetAllAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IEnumerable<Attendee>>(new[] { attendee }));

            var sut = CreateSut();

            var result = await sut.GetAllAttendeesAsync(CancellationToken.None);

            result.Should().HaveCount(1);
            result.First().Email.Should().Be("a@gmail.com");

            _queries.Verify(q => q.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAllInstructorsAsync_should_call_queries_and_map_result()
        {
            var i1 = CreateInstructor("teach@gmail.com");

            _queries.Setup(q => q.GetAllInstructorsAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IEnumerable<Instructor>>(new[] { i1 }));

            var sut = CreateSut();

            var result = (await sut.GetAllInstructorsAsync(CancellationToken.None)).ToList();

            result.Should().HaveCount(1);
            result[0].Email.Should().Be("teach@gmail.com");

            _queries.Verify(q => q.GetAllInstructorsAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAllStudentsAsync_should_call_queries_and_map_result()
        {
            var s1 = CreateStudent("student@gmail.com");

            _queries.Setup(q => q.GetAllStudentsAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IEnumerable<Student>>(new[] { (Student)s1 }));

            var sut = CreateSut();

            var result = (await sut.GetAllStudentsAsync(CancellationToken.None)).ToList();

            result.Should().HaveCount(1);
            result[0].Email.Should().Be("student@gmail.com");

            _queries.Verify(q => q.GetAllStudentsAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetInstructorsByCompetenceAsync_should_return_empty_when_none_found()
        {
            var competence = "Missing";

            _queries.Setup(q => q.GetInstructorsByCompetenceAsync(competence, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IEnumerable<Instructor>>(Array.Empty<Instructor>()));

            var sut = CreateSut();

            var result = await sut.GetInstructorsByCompetenceAsync(competence, CancellationToken.None);

            result.Should().BeEmpty();

            _queries.Verify(q => q.GetInstructorsByCompetenceAsync(competence, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetInstructorsByCompetenceAsync_should_call_queries_and_map_result()
        {
            var competence = "C#";
            var i1 = CreateInstructor("teach@gmail.com");

            _queries.Setup(q => q.GetInstructorsByCompetenceAsync(competence, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IEnumerable<Instructor>>(new[] { i1 }));

            var sut = CreateSut();

            var result = (await sut.GetInstructorsByCompetenceAsync(competence, CancellationToken.None)).ToList();

            result.Should().HaveCount(1);
            result[0].Email.Should().Be("teach@gmail.com");

            _queries.Verify(q => q.GetInstructorsByCompetenceAsync(competence, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SearchAttendeesByNameAsync_should_call_queries_and_map_result()
        {
            var term = "ada";
            var i1 = CreateInstructor("ada@gmail.com");

            _queries.Setup(q => q.SearchByNameAsync(term, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IEnumerable<Attendee>>(new Attendee[] { i1 }));

            var sut = CreateSut();

            var result = (await sut.SearchAttendeesByNameAsync(term, CancellationToken.None)).ToList();

            result.Should().HaveCount(1);
            result[0].Email.Should().Be("ada@gmail.com");

            _queries.Verify(q => q.SearchByNameAsync(term, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAttendeeByIdAsync_should_return_dto_when_found()
        {
            var attendee = CreateStudent("student@gmail.com");

            _repo.Setup(r => r.GetByIdAsync(It.IsAny<AttendeeId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(attendee);

            var sut = CreateSut();

            var result = await sut.GetAttendeeByIdAsync(Guid.NewGuid(), CancellationToken.None);

            result.Should().BeOfType<StudentDTO>();
            result.Email.Should().Be("student@gmail.com");
        }

        [Fact]
        public async Task GetAttendeesByFirstNameAsync_should_call_queries_and_return_result()
        {
            var name = "Ada";
            var instructor = CreateInstructor("ada@gmail.com");

            _queries.Setup(q => q.SearchByNameAsync(name, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IEnumerable<Attendee>>(new Attendee[] { instructor }));

            var sut = CreateSut();

            var result = (await sut.GetAttendeesByFirstNameAsync(name, CancellationToken.None)).ToList();

            result.Should().HaveCount(1);
            result[0].Email.Should().Be("ada@gmail.com");

            _queries.Verify(q => q.SearchByNameAsync(name, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAttendeesByLastNameAsync_should_call_queries_and_return_result()
        {
            var lastName = "Lovelace";
            var instructor = CreateInstructor("ada@gmail.com");

            _queries.Setup(q => q.SearchByNameAsync(lastName, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IEnumerable<Attendee>>(new Attendee[] { instructor }));

            var sut = CreateSut();

            var result = (await sut.GetAttendeesByLastNameAsync(lastName, CancellationToken.None)).ToList();

            result.Should().HaveCount(1);
            result[0].Email.Should().Be("ada@gmail.com");

            _queries.Verify(q => q.SearchByNameAsync(lastName, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAttendeesByFirstNameAsync_should_return_empty_when_none_found()
        {
            var name = "Missing";

            _queries.Setup(q => q.SearchByNameAsync(name, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IEnumerable<Attendee>>(Array.Empty<Attendee>()));

            var sut = CreateSut();

            var result = await sut.GetAttendeesByFirstNameAsync(name, CancellationToken.None);

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllAttendeesAsync_should_return_all_attendees()
        {
            var attendees = new List<Attendee>
    {
        CreateStudent("a@gmail.com"),
        CreateInstructor("b@gmail.com")
    };

            _queries.Setup(q => q.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(attendees);

            var sut = CreateSut();

            var result = await sut.GetAllAttendeesAsync(CancellationToken.None);

            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllInstructorsAsync_should_return_only_instructors()
        {
            var instructors = new List<Instructor>
    {
        CreateInstructor("inst@gmail.com")
    };

            _queries.Setup(q => q.GetAllInstructorsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(instructors);

            var sut = CreateSut();

            var result = await sut.GetAllInstructorsAsync(CancellationToken.None);

            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetAllStudentsAsync_should_return_only_students()
        {
            var students = new List<Student>
    {
        (Student)CreateStudent("student@gmail.com")
    };

            _queries.Setup(q => q.GetAllStudentsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(students);

            var sut = CreateSut();

            var result = await sut.GetAllStudentsAsync();

            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task SearchAttendeesByNameAsync_should_return_matches()
        {
            var attendees = new List<Attendee>
    {
        CreateStudent("john@gmail.com")
    };

            _queries.Setup(q => q.SearchByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(attendees);

            var sut = CreateSut();

            var result = await sut.SearchAttendeesByNameAsync("john", CancellationToken.None);

            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task SearchAttendeesByNameAsync_should_return_empty_when_none()
        {
            _queries.Setup(q => q.SearchByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Attendee>());

            var sut = CreateSut();

            var result = await sut.SearchAttendeesByNameAsync("missing", CancellationToken.None);

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAttendeesByFirstNameAsync_should_call_search()
        {
            var attendees = new List<Attendee>
    {
        CreateStudent("test@gmail.com")
    };

            _queries.Setup(q => q.SearchByNameAsync("John", It.IsAny<CancellationToken>()))
                .ReturnsAsync(attendees);

            var sut = CreateSut();

            var result = await sut.GetAttendeesByFirstNameAsync("John", CancellationToken.None);

            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetAttendeesByLastNameAsync_should_call_search()
        {
            var attendees = new List<Attendee>
    {
        CreateStudent("test@gmail.com")
    };

            _queries.Setup(q => q.SearchByNameAsync("Doe", It.IsAny<CancellationToken>()))
                .ReturnsAsync(attendees);

            var sut = CreateSut();

            var result = await sut.GetAttendeesByLastNameAsync("Doe", CancellationToken.None);

            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetInstructorsByCompetenceAsync_should_return_instructors()
        {
            var instructors = new List<Instructor>
    {
        CreateInstructor("inst@gmail.com")
    };

            _queries.Setup(q => q.GetInstructorsByCompetenceAsync("C#", It.IsAny<CancellationToken>()))
                .ReturnsAsync(instructors);

            var sut = CreateSut();

            var result = await sut.GetInstructorsByCompetenceAsync("C#", CancellationToken.None);

            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetAttendeeByIdAsync_should_return_attendee_when_found()
        {
            var attendee = CreateStudent("found@gmail.com");

            _repo.Setup(r => r.GetByIdAsync(It.IsAny<AttendeeId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(attendee);

            var sut = CreateSut();

            var result = await sut.GetAttendeeByIdAsync(Guid.NewGuid(), CancellationToken.None);

            result.Email.Should().Be("found@gmail.com");
        }

        [Fact]
        public async Task GetAttendeeByEmailAsync_should_return_attendee_when_found()
        {
            var attendee = CreateStudent("found@gmail.com");

            _repo.Setup(r => r.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(attendee);

            var sut = CreateSut();

            var result = await sut.GetAttendeeByEmailAsync("found@gmail.com", CancellationToken.None);

            result.Email.Should().Be("found@gmail.com");
        }
    }
}
