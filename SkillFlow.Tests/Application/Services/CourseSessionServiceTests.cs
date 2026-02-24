using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using SkillFlow.Application.Abstractions.Caching;
using SkillFlow.Application.DTOs.CourseSessions;
using SkillFlow.Application.Services.CourseSessions;
using SkillFlow.Domain.Attendees;
using SkillFlow.Domain.Courses;
using SkillFlow.Domain.Entities.Attendees;
using SkillFlow.Domain.Entities.Courses;
using SkillFlow.Domain.Entities.CourseSessions;
using SkillFlow.Domain.Entities.Locations;
using SkillFlow.Domain.Enums;
using SkillFlow.Domain.Exceptions;
using SkillFlow.Domain.Interfaces;
using System.Reflection;
using Xunit;

namespace SkillFlow.Tests.Application.Services
{
    public class CourseSessionServiceTests
    {
        private readonly Mock<ICourseSessionRepository> _sessionRepo = new();
        private readonly Mock<IAttendeeRepository> _attendeeRepo = new();
        private readonly Mock<ICourseRepository> _courseRepo = new();
        private readonly Mock<ILocationRepository> _locationRepo = new();
        private readonly Mock<IUnitOfWork> _uow = new();
        private readonly Mock<ICourseSessionCacheBuster> _cacheBuster = new();

        private CourseSessionService CreateSut()
            => new(
                _sessionRepo.Object,
                _attendeeRepo.Object,
                _courseRepo.Object,
                _locationRepo.Object,
                _uow.Object,
                _cacheBuster.Object
            );

        [Fact]
        public async Task AddInstructorToCourseSessionAsync_WhenSessionNotFound_ShouldThrow()
        {
            var sut = CreateSut();

            _sessionRepo
                .Setup(x => x.GetByIdAsync(It.IsAny<CourseSessionId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CourseSession?)null);

            var act = async () => await sut.AddInstructorToCourseSessionAsync(
                Guid.NewGuid(),
                Guid.NewGuid(),
                new byte[] { 1, 2, 3 },
                CancellationToken.None);

            await act.Should().ThrowAsync<CourseSessionNotFoundException>();

            _sessionRepo.Verify(x => x.UpdateAsync(It.IsAny<CourseSession>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()), Times.Never);
            _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task AddInstructorToCourseSessionAsync_WhenAttendeeNotFound_ShouldThrow()
        {
            var sut = CreateSut();

            _sessionRepo
                .Setup(x => x.GetByIdAsync(It.IsAny<CourseSessionId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateSessionGraph());

            _attendeeRepo
                .Setup(x => x.GetByIdAsync(It.IsAny<AttendeeId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Attendee?)null);

            var act = async () => await sut.AddInstructorToCourseSessionAsync(
                Guid.NewGuid(),
                Guid.NewGuid(),
                new byte[] { 1 },
                CancellationToken.None);

            await act.Should().ThrowAsync<AttendeeNotFoundException>();

            _sessionRepo.Verify(x => x.UpdateAsync(It.IsAny<CourseSession>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()), Times.Never);
            _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task AddInstructorToCourseSessionAsync_WhenAttendeeIsNotInstructor_ShouldThrowInvalidRole()
        {
            var sut = CreateSut();

            _sessionRepo
                .Setup(x => x.GetByIdAsync(It.IsAny<CourseSessionId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateSessionGraph());

            _attendeeRepo
                .Setup(x => x.GetByIdAsync(It.IsAny<AttendeeId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateStudent());

            var act = async () => await sut.AddInstructorToCourseSessionAsync(
                Guid.NewGuid(),
                Guid.NewGuid(),
                new byte[] { 1 },
                CancellationToken.None);

            await act.Should().ThrowAsync<InvalidRoleException>();

            _sessionRepo.Verify(x => x.UpdateAsync(It.IsAny<CourseSession>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()), Times.Never);
            _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task AddInstructorToCourseSessionAsync_HappyPath_ShouldUpdateAndSave()
        {
            var sut = CreateSut();

            var session = CreateSessionGraph();
            var instructor = CreateInstructor();

            _sessionRepo
                .Setup(x => x.GetByIdAsync(It.IsAny<CourseSessionId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(session);

            _attendeeRepo
                .Setup(x => x.GetByIdAsync(It.IsAny<AttendeeId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(instructor);

            var rowVersion = new byte[] { 9, 9, 9 };

            await sut.AddInstructorToCourseSessionAsync(Guid.NewGuid(), Guid.NewGuid(), rowVersion, CancellationToken.None);

            _sessionRepo.Verify(x => x.UpdateAsync(session, rowVersion, It.IsAny<CancellationToken>()), Times.Once);
            _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        // ---------------------------
        // CreateCourseSessionAsync
        // ---------------------------

        [Fact]
        public async Task CreateCourseSessionAsync_WhenNoInstructorIds_ShouldThrow()
        {
            var sut = CreateSut();

            var dto = new CreateCourseSessionDTO
            {
                InstructorIds = [],
                CourseCode = "MTHBAS-010",
                LocationName = "Stockholm",
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(2),
                Capacity = 10
            };

            var act = async () => await sut.CreateCourseSessionAsync(dto, CancellationToken.None);

            await act.Should().ThrowAsync<InstructorIsRequiredException>();

            _sessionRepo.Verify(x => x.AddAsync(It.IsAny<CourseSession>(), It.IsAny<CancellationToken>()), Times.Never);
            _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreateCourseSessionAsync_WhenCourseNotFound_ShouldThrow()
        {
            var sut = CreateSut();
            var dto = ValidCreateDto();

            _courseRepo
                .Setup(x => x.GetByCourseCodeAsync(It.IsAny<CourseCode>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Course?)null);

            var act = async () => await sut.CreateCourseSessionAsync(dto, CancellationToken.None);

            await act.Should().ThrowAsync<CourseNotFoundException>();

            _sessionRepo.Verify(x => x.AddAsync(It.IsAny<CourseSession>(), It.IsAny<CancellationToken>()), Times.Never);
            _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreateCourseSessionAsync_WhenLocationNotFound_ShouldThrow()
        {
            var sut = CreateSut();
            var dto = ValidCreateDto();

            _courseRepo
                .Setup(x => x.GetByCourseCodeAsync(It.IsAny<CourseCode>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateCourse());

            _locationRepo
                .Setup(x => x.GetByLocationNameAsync(It.IsAny<LocationName>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Location?)null);

            var act = async () => await sut.CreateCourseSessionAsync(dto, CancellationToken.None);

            await act.Should().ThrowAsync<LocationNotFoundException>();

            _sessionRepo.Verify(x => x.AddAsync(It.IsAny<CourseSession>(), It.IsAny<CancellationToken>()), Times.Never);
            _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreateCourseSessionAsync_WhenAttendeeNotFound_ShouldThrow()
        {
            var sut = CreateSut();
            var dto = ValidCreateDto();

            _courseRepo
                .Setup(x => x.GetByCourseCodeAsync(It.IsAny<CourseCode>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateCourse());

            _locationRepo
                .Setup(x => x.GetByLocationNameAsync(It.IsAny<LocationName>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateLocation());

            _attendeeRepo
                .Setup(x => x.GetByIdAsync(It.IsAny<AttendeeId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Attendee?)null);

            var act = async () => await sut.CreateCourseSessionAsync(dto, CancellationToken.None);

            await act.Should().ThrowAsync<AttendeeNotFoundException>();

            _sessionRepo.Verify(x => x.AddAsync(It.IsAny<CourseSession>(), It.IsAny<CancellationToken>()), Times.Never);
            _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreateCourseSessionAsync_WhenAttendeeIsNotInstructor_ShouldThrowInvalidRole()
        {
            var sut = CreateSut();
            var dto = ValidCreateDto();

            _courseRepo
                .Setup(x => x.GetByCourseCodeAsync(It.IsAny<CourseCode>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateCourse());

            _locationRepo
                .Setup(x => x.GetByLocationNameAsync(It.IsAny<LocationName>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateLocation());

            _attendeeRepo
                .Setup(x => x.GetByIdAsync(It.IsAny<AttendeeId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateStudent());

            var act = async () => await sut.CreateCourseSessionAsync(dto, CancellationToken.None);

            await act.Should().ThrowAsync<InvalidRoleException>();

            _sessionRepo.Verify(x => x.AddAsync(It.IsAny<CourseSession>(), It.IsAny<CancellationToken>()), Times.Never);
            _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreateCourseSessionAsync_ShouldDistinctInstructorIds()
        {
            var sut = CreateSut();
            var dto = ValidCreateDto();

            var same = dto.InstructorIds[0];
            dto.InstructorIds.Add(same); // dubblett

            _courseRepo
                .Setup(x => x.GetByCourseCodeAsync(It.IsAny<CourseCode>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateCourse());

            _locationRepo
                .Setup(x => x.GetByLocationNameAsync(It.IsAny<LocationName>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateLocation());

            _attendeeRepo
                .Setup(x => x.GetByIdAsync(It.IsAny<AttendeeId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateInstructor());

            // Returnerar en "graph" så MapToDTO inte kraschar
            _sessionRepo
                .Setup(x => x.GetByIdWithInstructorsAndEnrollmentsAsync(It.IsAny<CourseSessionId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateSessionGraph());

            await sut.CreateCourseSessionAsync(dto, CancellationToken.None);

            _attendeeRepo.Verify(x =>
                x.GetByIdAsync(It.Is<AttendeeId>(a => a.Value == same), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task CreateCourseSessionAsync_HappyPath_ShouldAdd_Save_AndReturnDto()
        {
            var sut = CreateSut();
            var dto = ValidCreateDto();

            _courseRepo
                .Setup(x => x.GetByCourseCodeAsync(It.IsAny<CourseCode>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateCourse());

            _locationRepo
                .Setup(x => x.GetByLocationNameAsync(It.IsAny<LocationName>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateLocation());

            _attendeeRepo
                .Setup(x => x.GetByIdAsync(It.IsAny<AttendeeId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateInstructor());

            CourseSession? added = null;

            _sessionRepo
                .Setup(x => x.AddAsync(It.IsAny<CourseSession>(), It.IsAny<CancellationToken>()))
                .Callback<CourseSession, CancellationToken>((s, _) => added = s)
                .Returns(Task.CompletedTask);

            // service returnerar via GetCourseSessionByIdAsync => repo.GetByIdWithInstructorsAndEnrollmentsAsync
            _sessionRepo
                .Setup(x => x.GetByIdWithInstructorsAndEnrollmentsAsync(It.IsAny<CourseSessionId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                {
                    // Skapa en sessiongraph som MapToDTO kan mappa
                    var graph = CreateSessionGraph();
                    // Om vi lyckades fånga "added", kan vi spegla id/capacity/dates (valfritt)
                    if (added is not null)
                    {
                        // För att DTO ska se rimlig ut
                        SetBackingField(graph, "Capacity", added.Capacity);
                        SetBackingField(graph, "StartDate", added.StartDate);
                        SetBackingField(graph, "EndDate", added.EndDate);
                        SetBackingField(graph, "CourseId", added.CourseId);
                        SetBackingField(graph, "CourseCode", added.CourseCode);
                        SetBackingField(graph, "LocationId", added.LocationId);
                    }
                    return graph;
                });

            var result = await sut.CreateCourseSessionAsync(dto, CancellationToken.None);

            result.Should().NotBeNull();
            result.Capacity.Should().Be(dto.Capacity);

            _sessionRepo.Verify(x => x.AddAsync(It.IsAny<CourseSession>(), It.IsAny<CancellationToken>()), Times.Once);
            _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        // ---------------------------
        // DeleteCourseSessionAsync
        // ---------------------------

        [Fact]
        public async Task DeleteCourseSessionAsync_WhenDeleteReturnsFalse_ShouldThrow()
        {
            var sut = CreateSut();

            _sessionRepo
                .Setup(x => x.DeleteAsync(It.IsAny<CourseSessionId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var act = async () => await sut.DeleteCourseSessionAsync(Guid.NewGuid(), CancellationToken.None);

            await act.Should().ThrowAsync<CourseSessionNotFoundException>();

            _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteCourseSessionAsync_WhenDeleteReturnsTrue_ShouldSaveChanges()
        {
            var sut = CreateSut();

            _sessionRepo
                .Setup(x => x.DeleteAsync(It.IsAny<CourseSessionId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            await sut.DeleteCourseSessionAsync(Guid.NewGuid(), CancellationToken.None);

            _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        // ---------------------------
        // EnrollStudentAsync
        // ---------------------------

        [Fact]
        public async Task EnrollStudentAsync_WhenSessionNotFound_ShouldThrow()
        {
            var sut = CreateSut();

            _sessionRepo
                .Setup(x => x.GetByIdWithInstructorsAndEnrollmentsAsync(It.IsAny<CourseSessionId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CourseSession?)null);

            var act = async () => await sut.EnrollStudentAsync(Guid.NewGuid(), Guid.NewGuid(), new byte[] { 1 }, CancellationToken.None);

            await act.Should().ThrowAsync<CourseSessionNotFoundException>();
        }

        [Fact]
        public async Task EnrollStudentAsync_WhenAttendeeNotFound_ShouldThrow()
        {
            var sut = CreateSut();

            _sessionRepo
                .Setup(x => x.GetByIdWithInstructorsAndEnrollmentsAsync(It.IsAny<CourseSessionId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateSessionWithInstructorGraph());

            _attendeeRepo
                .Setup(x => x.GetByIdAsync(It.IsAny<AttendeeId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Attendee?)null);

            var act = async () => await sut.EnrollStudentAsync(Guid.NewGuid(), Guid.NewGuid(), new byte[] { 1 }, CancellationToken.None);

            await act.Should().ThrowAsync<AttendeeNotFoundException>();
        }

        [Fact]
        public async Task EnrollStudentAsync_WhenAttendeeIsNotStudent_ShouldThrowInvalidRole()
        {
            var sut = CreateSut();

            _sessionRepo
                .Setup(x => x.GetByIdWithInstructorsAndEnrollmentsAsync(It.IsAny<CourseSessionId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateSessionWithInstructorGraph());

            _attendeeRepo
                .Setup(x => x.GetByIdAsync(It.IsAny<AttendeeId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateInstructor());

            var act = async () => await sut.EnrollStudentAsync(Guid.NewGuid(), Guid.NewGuid(), new byte[] { 1 }, CancellationToken.None);

            await act.Should().ThrowAsync<InvalidRoleException>();
        }

        [Fact]
        public async Task EnrollStudentAsync_HappyPath_ShouldSaveChanges()
        {
            var sut = CreateSut();

            _sessionRepo
                .Setup(x => x.GetByIdWithInstructorsAndEnrollmentsAsync(It.IsAny<CourseSessionId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateSessionWithInstructorGraph());

            _attendeeRepo
                .Setup(x => x.GetByIdAsync(It.IsAny<AttendeeId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateStudent());

            await sut.EnrollStudentAsync(Guid.NewGuid(), Guid.NewGuid(), new byte[] { 1 }, CancellationToken.None);

            _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            // Notera: metoden kallar INTE sessionRepo.UpdateAsync här
            _sessionRepo.Verify(x => x.UpdateAsync(It.IsAny<CourseSession>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        // ---------------------------
        // SetEnrollmentStatusAsync (transaction + concurrency)
        // ---------------------------

        [Fact]
        public async Task SetEnrollmentStatusAsync_OnConcurrency_ShouldRollback_AndThrowConcurrencyException()
        {
            var sut = CreateSut();

            var tx = new Mock<ITransaction>(MockBehavior.Strict);
            tx.Setup(x => x.RollbackAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            tx.Setup(x => x.DisposeAsync()).Returns(ValueTask.CompletedTask);
            tx.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            _uow
                .Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(tx.Object);

            var session = CreateSessionWithInstructorAndStudentEnrollmentGraph();

            _sessionRepo
                .Setup(x => x.GetByIdWithInstructorsAndEnrollmentsAsync(It.IsAny<CourseSessionId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(session);

            _sessionRepo
                .Setup(x => x.UpdateAsync(It.IsAny<CourseSession>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _uow
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ConcurrencyException());

            var studentId = session.Enrollments.First().StudentId.Value;

            var act = async () => await sut.SetEnrollmentStatusAsync(
                session.Id.Value,
                studentId,
                EnrollmentStatus.Approved,
                new byte[] { 1 },
                CancellationToken.None);

            await act.Should().ThrowAsync<ConcurrencyException>();

            tx.Verify(x => x.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
            tx.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task SetEnrollmentStatusAsync_HappyPath_ShouldCommit()
        {
            var sut = CreateSut();

            var tx = new Mock<ITransaction>(MockBehavior.Strict);
            tx.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            tx.Setup(x => x.RollbackAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            tx.Setup(x => x.DisposeAsync()).Returns(ValueTask.CompletedTask);

            _uow
                .Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(tx.Object);

            var session = CreateSessionWithInstructorAndStudentEnrollmentGraph();

            _sessionRepo
                .Setup(x => x.GetByIdWithInstructorsAndEnrollmentsAsync(It.IsAny<CourseSessionId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(session);

            _sessionRepo
                .Setup(x => x.UpdateAsync(It.IsAny<CourseSession>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _uow
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            var studentId = session.Enrollments.First().StudentId.Value;

            await sut.SetEnrollmentStatusAsync(
                session.Id.Value,
                studentId,
                EnrollmentStatus.Approved,
                new byte[] { 1 },
                CancellationToken.None);

            _sessionRepo.Verify(x => x.UpdateAsync(session, It.IsAny<byte[]>(), It.IsAny<CancellationToken>()), Times.Once);
            _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            tx.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            tx.Verify(x => x.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        // ---------------------------
        // UpdateCourseSessionAsync
        // ---------------------------

        [Fact]
        public async Task UpdateCourseSessionAsync_WhenSessionNotFound_ShouldThrow()
        {
            var sut = CreateSut();

            _sessionRepo
                .Setup(x => x.GetByIdWithInstructorsAndEnrollmentsAsync(It.IsAny<CourseSessionId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CourseSession?)null);

            var dto = new UpdateCourseSessionDTO
            {
                Capacity = 20,
                RowVersion = new byte[] { 1 }
            };

            var act = async () => await sut.UpdateCourseSessionAsync(Guid.NewGuid(), dto, CancellationToken.None);

            await act.Should().ThrowAsync<CourseSessionNotFoundException>();
        }

        [Fact]
        public async Task UpdateCourseSessionAsync_WhenCapacityProvided_ShouldUpdate_AndSave()
        {
            var sut = CreateSut();

            var session = CreateSessionGraph();

            _sessionRepo
                .Setup(x => x.GetByIdWithInstructorsAndEnrollmentsAsync(It.IsAny<CourseSessionId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(session);

            _sessionRepo
                .Setup(x => x.UpdateAsync(It.IsAny<CourseSession>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var dto = new UpdateCourseSessionDTO
            {
                Capacity = 20,
                RowVersion = new byte[] { 7, 7 }
            };

            var result = await sut.UpdateCourseSessionAsync(session.Id.Value, dto, CancellationToken.None);

            result.Capacity.Should().Be(20);

            _sessionRepo.Verify(x => x.UpdateAsync(session, dto.RowVersion, It.IsAny<CancellationToken>()), Times.Once);
            _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateCourseSessionAsync_WhenDatesProvided_ShouldUpdate_AndSave()
        {
            var sut = CreateSut();

            var session = CreateSessionGraph();

            _sessionRepo
                .Setup(x => x.GetByIdWithInstructorsAndEnrollmentsAsync(It.IsAny<CourseSessionId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(session);

            _sessionRepo
                .Setup(x => x.UpdateAsync(It.IsAny<CourseSession>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var newStart = DateTime.UtcNow.AddDays(10);
            var newEnd = DateTime.UtcNow.AddDays(11);

            var dto = new UpdateCourseSessionDTO
            {
                StartDate = newStart,
                EndDate = newEnd,
                RowVersion = new byte[] { 1 }
            };

            var result = await sut.UpdateCourseSessionAsync(session.Id.Value, dto, CancellationToken.None);

            // DTO-mappning gör ToLocalTime() - så här jämför vi på "value" nivån (datum/tid kan skifta på maskinen)
            result.StartDate.Should().Be(newStart.ToLocalTime());
            result.EndDate.Should().Be(newEnd.ToLocalTime());

            _sessionRepo.Verify(x => x.UpdateAsync(session, dto.RowVersion, It.IsAny<CancellationToken>()), Times.Once);
            _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        // ---------------------------
        // Helpers: DTO + Entities + Reflection "seams"
        // ---------------------------

        private static string ValidCourseCodeValue()
        {
            var type = Enum.GetValues<CourseType>().First();   // tar första enum-värdet som finns
            return CourseCode.Create("Math", type, 10).Value;
        }

        private static CreateCourseSessionDTO ValidCreateDto()
            => new()
            {
                InstructorIds = new List<Guid> { Guid.NewGuid() },
                CourseCode = "MTHBAS-010",
                LocationName = "Stockholm",
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(2),
                Capacity = 10
            };

        private static Course CreateCourse()
            => Course.Create(
                CourseCode.Create("Math", CourseType.BAS, 10),
                CourseName.Create("Math"),
                CourseDescription.Create("Desc"));

        private static Location CreateLocation()
            => Location.Create(LocationName.Create("Stockholm"));

        private static Instructor CreateInstructor()
            => (Instructor)Attendee.CreateInstructor(
                Email.Create(Guid.NewGuid() + "@test.com"),
                AttendeeName.Create("Ada", "Lovelace"),
                null);

        private static Student CreateStudent()
            => (Student)Attendee.CreateStudent(
                Email.Create(Guid.NewGuid() + "@test.com"),
                AttendeeName.Create("Test", "Student"),
                null);

        private static CourseSession CreateBaseSession()
            => CourseSession.Create(
                CourseSessionId.New(),
                CourseId.New(),
                CourseCode.Create("Math", CourseType.BAS, 10),
                DateTime.UtcNow.AddDays(1),
                DateTime.UtcNow.AddDays(2),
                10,
                LocationId.New());

        /// <summary>
        /// Skapar en session och "sätter" navigation properties Course/Location via reflection
        /// så MapToDTO inte kraschar i unit tests.
        /// </summary>
        private static CourseSession CreateSessionGraph()
        {
            var course = CreateCourse();
            var location = CreateLocation();

            var session = CourseSession.Create(
                CourseSessionId.New(),
                course.Id,
                course.CourseCode,
                DateTime.UtcNow.AddDays(1),
                DateTime.UtcNow.AddDays(2),
                10,
                location.Id);

            SetBackingField(session, "Course", course);
            SetBackingField(session, "Location", location);

            // RowVersion behövs ibland i DTO, men kan vara null i unit test annars.
            //SetBackingField(session, "RowVersion", new byte[] { 1, 0, 0, 0 });

            return session;
        }

        private static CourseSession CreateSessionWithInstructorGraph()
        {
            var session = CreateSessionGraph();
            session.AddInstructor(CreateInstructor());
            return session;
        }

        private static CourseSession CreateSessionWithInstructorAndStudentEnrollmentGraph()
        {
            var session = CreateSessionWithInstructorGraph();

            var student = CreateStudent();
            session.AddStudent(student);

            // Enrollment.Student är ORM-navigation (private set). Sätt via reflection så ev mapping kan använda den.
            foreach (var e in session.Enrollments)
            {
                SetBackingField(e, "Student", student);
            }

            return session;
        }

        /// <summary>
        /// Sätter auto-property backing field: "<PropertyName>k__BackingField"
        /// Funkar för private set/init properties.
        /// </summary>
        private static void SetBackingField(object target, string propertyName, object? value)
        {
            var fieldName = $"<{propertyName}>k__BackingField";
            var field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);

            if (field is null)
                throw new InvalidOperationException($"Could not find backing field '{fieldName}' on '{target.GetType().Name}'");

            field.SetValue(target, value);
        }

        /// <summary>
        /// Overload för value types (int/DateTime/structs etc).
        /// </summary>
        private static void SetBackingField<T>(object target, string propertyName, T value)
        {
            SetBackingField(target, propertyName, (object?)value);
        }
    }
}
