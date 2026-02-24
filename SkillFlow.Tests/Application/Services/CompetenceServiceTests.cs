using FluentAssertions;
using Moq;
using SkillFlow.Application.Abstractions.Caching;
using SkillFlow.Application.DTOs.Competences;
using SkillFlow.Application.Services.Competences;
using SkillFlow.Domain.Attendees;
using SkillFlow.Domain.Entities.Attendees;
using SkillFlow.Domain.Entities.Competences;
using SkillFlow.Domain.Exceptions;
using SkillFlow.Domain.Interfaces;
using Xunit;

namespace SkillFlow.Tests.Application.Services
{
    public class CompetenceServiceTests
    {
        private readonly Mock<ICompetenceRepository> _repo = new();
        private readonly Mock<IUnitOfWork> _uow = new();
        private readonly Mock<ICompetenceCacheBuster> _cacheBuster = new();

        private CompetenceService CreateSut()
            => new(_repo.Object, _uow.Object, _cacheBuster.Object);

        // -------------------------
        // CreateCompetenceAsync
        // -------------------------

        [Fact]
        public async Task CreateCompetenceAsync_WhenNameExists_ShouldThrow_AndNotAddOrSave()
        {
            var sut = CreateSut();
            var dto = new CreateCompetenceDTO { Name = "C#" };

            _repo.Setup(x => x.ExistsByNameAsync(It.IsAny<CompetenceName>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(true);

            var act = async () => await sut.CreateCompetenceAsync(dto, CancellationToken.None);

            await act.Should().ThrowAsync<CompetenceNameAllreadyExistsException>();

            _repo.Verify(x => x.AddAsync(It.IsAny<Competence>(), It.IsAny<CancellationToken>()), Times.Never);
            _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreateCompetenceAsync_HappyPath_ShouldAdd_Save_AndReturnDto()
        {
            var sut = CreateSut();
            var dto = new CreateCompetenceDTO { Name = "backend development" }; // test normalisering också

            _repo.Setup(x => x.ExistsByNameAsync(It.IsAny<CompetenceName>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(false);

            Competence? added = null;

            _repo.Setup(x => x.AddAsync(It.IsAny<Competence>(), It.IsAny<CancellationToken>()))
                 .Callback<Competence, CancellationToken>((c, _) => added = c)
                 .Returns(Task.CompletedTask);

            _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await sut.CreateCompetenceAsync(dto, CancellationToken.None);

            _repo.Verify(x => x.AddAsync(It.IsAny<Competence>(), It.IsAny<CancellationToken>()), Times.Once);
            _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            result.Should().NotBeNull();
            result.Id.Should().NotBe(Guid.Empty);

            // Name är normaliserat via NormalizeName()
            result.Name.Should().Be(CompetenceName.Create(dto.Name).Value);

            // Vi kan också verifiera att det som addades hade rätt name
            added.Should().NotBeNull();
            added!.Name.Should().Be(CompetenceName.Create(dto.Name));
        }

        // -------------------------
        // DeleteCompetenceAsync
        // -------------------------

        [Fact]
        public async Task DeleteCompetenceAsync_WhenDeleteReturnsFalse_ShouldThrow_AndNotSave()
        {
            var sut = CreateSut();

            _repo.Setup(x => x.DeleteAsync(It.IsAny<CompetenceId>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(false);

            var act = async () => await sut.DeleteCompetenceAsync(Guid.NewGuid(), CancellationToken.None);

            await act.Should().ThrowAsync<CompetenceNotFoundException>();

            _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteCompetenceAsync_WhenDeleteReturnsTrue_ShouldSave()
        {
            var sut = CreateSut();

            _repo.Setup(x => x.DeleteAsync(It.IsAny<CompetenceId>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(true);

            _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            await sut.DeleteCompetenceAsync(Guid.NewGuid(), CancellationToken.None);

            _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        // -------------------------
        // GetAllCompetencesAsync
        // -------------------------

        [Fact]
        public async Task GetAllCompetencesAsync_ShouldReturnMappedDetails()
        {
            var sut = CreateSut();

            var competence = Competence.Create(CompetenceName.Create("C#"));
            var instructor = CreateInstructor();

            // Lägg till instructor i competence (domain method)
            competence.AddInstructor(instructor);

            _repo.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new List<Competence> { competence });

            var result = (await sut.GetAllCompetencesAsync(CancellationToken.None)).ToList();

            result.Should().HaveCount(1);
            result[0].Id.Should().Be(competence.Id.Value);
            result[0].Name.Should().Be(competence.Name.Value);
            result[0].Instructors.Should().HaveCount(1);
            result[0].Instructors[0].Id.Should().Be(instructor.Id.Value);
        }

        // -------------------------
        // GetCompetenceDetailsAsync
        // -------------------------

        [Fact]
        public async Task GetCompetenceDetailsAsync_WhenNotFound_ShouldThrow()
        {
            var sut = CreateSut();

            _repo.Setup(x => x.GetByIdAsync(It.IsAny<CompetenceId>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync((Competence?)null);

            var act = async () => await sut.GetCompetenceDetailsAsync(Guid.NewGuid(), CancellationToken.None);

            await act.Should().ThrowAsync<CompetenceNotFoundException>();
        }

        [Fact]
        public async Task GetCompetenceDetailsAsync_WhenFound_ShouldReturnMappedDetails()
        {
            var sut = CreateSut();

            var competence = Competence.Create(CompetenceName.Create("Dotnet"));
            var instructor = CreateInstructor();
            competence.AddInstructor(instructor);

            _repo.Setup(x => x.GetByIdAsync(It.IsAny<CompetenceId>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(competence);

            var result = await sut.GetCompetenceDetailsAsync(Guid.NewGuid(), CancellationToken.None);

            result.Id.Should().Be(competence.Id.Value);
            result.Name.Should().Be(competence.Name.Value);
            result.Instructors.Should().HaveCount(1);
            result.Instructors[0].Email.Should().Be(instructor.Email.Value);
        }

        // -------------------------
        // UpdateCompetenceAsync
        // -------------------------

        [Fact]
        public async Task UpdateCompetenceAsync_WhenNotFound_ShouldThrow()
        {
            var sut = CreateSut();

            _repo.Setup(x => x.GetByIdAsync(It.IsAny<CompetenceId>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync((Competence?)null);

            var dto = new UpdateCompetenceDTO { Name = "C#", RowVersion = new byte[] { 1 } };

            var act = async () => await sut.UpdateCompetenceAsync(Guid.NewGuid(), dto, CancellationToken.None);

            await act.Should().ThrowAsync<CompetenceNotFoundException>();

            _repo.Verify(x => x.UpdateAsync(It.IsAny<Competence>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()), Times.Never);
            _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdateCompetenceAsync_WhenNewNameExistsAndDifferent_ShouldThrow_AndNotUpdateOrSave()
        {
            var sut = CreateSut();

            var competence = Competence.Create(CompetenceName.Create("C#"));

            _repo.Setup(x => x.GetByIdAsync(It.IsAny<CompetenceId>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(competence);

            // Name finns redan
            _repo.Setup(x => x.ExistsByNameAsync(It.IsAny<CompetenceName>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(true);

            var dto = new UpdateCompetenceDTO { Name = "Dotnet", RowVersion = new byte[] { 1 } };

            var act = async () => await sut.UpdateCompetenceAsync(Guid.NewGuid(), dto, CancellationToken.None);

            await act.Should().ThrowAsync<CompetenceNameAllreadyExistsException>();

            _repo.Verify(x => x.UpdateAsync(It.IsAny<Competence>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()), Times.Never);
            _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdateCompetenceAsync_WhenNameIsSame_ShouldNotCheckExistsByName()
        {
            var sut = CreateSut();

            var competence = Competence.Create(CompetenceName.Create("Backend Development"));

            _repo.Setup(x => x.GetByIdAsync(It.IsAny<CompetenceId>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(competence);

            var dto = new UpdateCompetenceDTO
            {
                // annan casing / whitespace men normaliseras till samma
                Name = "   backend    development  ",
                RowVersion = new byte[] { 1 }
            };

            _repo.Setup(x => x.UpdateAsync(It.IsAny<Competence>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()))
                 .Returns(Task.CompletedTask);

            _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await sut.UpdateCompetenceAsync(Guid.NewGuid(), dto, CancellationToken.None);

            // ExistsByNameAsync ska INTE köras om namnet är samma efter normalisering
            _repo.Verify(x => x.ExistsByNameAsync(It.IsAny<CompetenceName>(), It.IsAny<CancellationToken>()), Times.Never);

            _repo.Verify(x => x.UpdateAsync(competence, dto.RowVersion, It.IsAny<CancellationToken>()), Times.Once);
            _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            result.Name.Should().Be(competence.Name.Value);
        }

        [Fact]
        public async Task UpdateCompetenceAsync_HappyPath_ShouldUpdate_Save_AndReturnDto()
        {
            var sut = CreateSut();

            var competence = Competence.Create(CompetenceName.Create("C#"));

            _repo.Setup(x => x.GetByIdAsync(It.IsAny<CompetenceId>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(competence);

            _repo.Setup(x => x.ExistsByNameAsync(It.IsAny<CompetenceName>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(false);

            _repo.Setup(x => x.UpdateAsync(It.IsAny<Competence>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()))
                 .Returns(Task.CompletedTask);

            _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var dto = new UpdateCompetenceDTO { Name = "Dotnet", RowVersion = new byte[] { 9 } };

            var result = await sut.UpdateCompetenceAsync(Guid.NewGuid(), dto, CancellationToken.None);

            competence.Name.Should().Be(CompetenceName.Create("Dotnet"));

            _repo.Verify(x => x.UpdateAsync(competence, dto.RowVersion, It.IsAny<CancellationToken>()), Times.Once);
            _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            result.Id.Should().Be(competence.Id.Value);
            result.Name.Should().Be(competence.Name.Value);
        }

        // -------------------------
        // Helpers
        // -------------------------

        private static Instructor CreateInstructor()
            => (Instructor)Attendee.CreateInstructor(
                Email.Create(Guid.NewGuid() + "@test.com"),
                AttendeeName.Create("Ada", "Lovelace"),
                null);
    }
}
