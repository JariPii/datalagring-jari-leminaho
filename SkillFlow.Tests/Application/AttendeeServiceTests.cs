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

namespace SkillFlow.Tests.Application
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
        public async Task AddCompetenceToInstructorAsync_should_rollback_on_exception()
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
    }
}
