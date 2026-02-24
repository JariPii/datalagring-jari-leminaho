using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using SkillFlow.Application.DTOs.Locations;
using SkillFlow.Application.Services.Locations;
using SkillFlow.Domain.Entities.Locations;
using SkillFlow.Domain.Exceptions;
using SkillFlow.Domain.Interfaces;
using Xunit;

namespace SkillFlow.Tests.Application.Services
{
    public class LocationServiceTests
    {
        private readonly Mock<ILocationRepository> _repo = new();
        private readonly Mock<IUnitOfWork> _uow = new();

        private LocationService CreateSut() => new(_repo.Object, _uow.Object);

        // -------------------------
        // CreateLocationAsync
        // -------------------------

        [Fact]
        public async Task CreateLocationAsync_WhenNameExists_ShouldThrow_AndNotAddOrSave()
        {
            var sut = CreateSut();
            var dto = new CreateLocationDTO { LocationName = "Stockholm" };

            _repo.Setup(x => x.ExistsByNameAsync(It.IsAny<LocationName>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(true);

            var act = async () => await sut.CreateLocationAsync(dto, CancellationToken.None);

            await act.Should().ThrowAsync<LocationNameAllreadyExistsException>();

            _repo.Verify(x => x.AddAsync(It.IsAny<Location>(), It.IsAny<CancellationToken>()), Times.Never);
            _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreateLocationAsync_HappyPath_ShouldAdd_Save_AndReturnDto()
        {
            var sut = CreateSut();
            var dto = new CreateLocationDTO { LocationName = "   göteborg   " };

            _repo.Setup(x => x.ExistsByNameAsync(It.IsAny<LocationName>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(false);

            Location? added = null;

            _repo.Setup(x => x.AddAsync(It.IsAny<Location>(), It.IsAny<CancellationToken>()))
                 .Callback<Location, CancellationToken>((l, _) => added = l)
                 .Returns(Task.CompletedTask);

            _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await sut.CreateLocationAsync(dto, CancellationToken.None);

            _repo.Verify(x => x.AddAsync(It.IsAny<Location>(), It.IsAny<CancellationToken>()), Times.Once);
            _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            result.Should().NotBeNull();
            result.Id.Should().NotBe(Guid.Empty);

            // Name normaliseras via NormalizeName()
            result.LocationName.Should().Be(LocationName.Create(dto.LocationName).Value);

            added.Should().NotBeNull();
            added!.LocationName.Should().Be(LocationName.Create(dto.LocationName));
        }

        // -------------------------
        // DeleteLocationAsync
        // -------------------------

        [Fact]
        public async Task DeleteLocationAsync_WhenNotFound_ShouldThrow()
        {
            var sut = CreateSut();

            _repo.Setup(x => x.GetByIdAsync(It.IsAny<LocationId>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync((Location?)null);

            var act = async () => await sut.DeleteLocationAsync(Guid.NewGuid(), CancellationToken.None);

            await act.Should().ThrowAsync<LocationNotFoundException>();

            _repo.Verify(x => x.Remove(It.IsAny<Location>()), Times.Never);
            _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteLocationAsync_HappyPath_ShouldRemove_AndSave()
        {
            var sut = CreateSut();
            var location = CreateLocation("Stockholm");

            _repo.Setup(x => x.GetByIdAsync(It.IsAny<LocationId>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(location);

            _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            await sut.DeleteLocationAsync(Guid.NewGuid(), CancellationToken.None);

            _repo.Verify(x => x.Remove(location), Times.Once);
            _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteLocationAsync_WhenDbUpdateException_ShouldThrowLocationInUseException()
        {
            var sut = CreateSut();
            var location = CreateLocation("Stockholm");

            _repo.Setup(x => x.GetByIdAsync(It.IsAny<LocationId>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(location);

            _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new DbUpdateException("FK"));

            var act = async () => await sut.DeleteLocationAsync(Guid.NewGuid(), CancellationToken.None);

            await act.Should().ThrowAsync<LocationInUseException>();

            _repo.Verify(x => x.Remove(location), Times.Once);
        }

        // -------------------------
        // GetAllLocationsAsync
        // -------------------------

        [Fact]
        public async Task GetAllLocationsAsync_ShouldReturnMappedDtos()
        {
            var sut = CreateSut();

            var locations = new List<Location>
            {
                CreateLocation("Stockholm"),
                CreateLocation("Göteborg")
            };

            _repo.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                 .ReturnsAsync(locations);

            var result = (await sut.GetAllLocationsAsync(CancellationToken.None)).ToList();

            result.Should().HaveCount(2);
            result.Select(x => x.LocationName).Should().Contain(new[] { "Stockholm", "Göteborg" });
        }

        // -------------------------
        // GetLocationByIdAsync
        // -------------------------

        [Fact]
        public async Task GetLocationByIdAsync_WhenNotFound_ShouldThrow()
        {
            var sut = CreateSut();

            _repo.Setup(x => x.GetByIdAsync(It.IsAny<LocationId>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync((Location?)null);

            var act = async () => await sut.GetLocationByIdAsync(Guid.NewGuid(), CancellationToken.None);

            await act.Should().ThrowAsync<LocationNotFoundException>();
        }

        [Fact]
        public async Task GetLocationByIdAsync_WhenFound_ShouldReturnDto()
        {
            var sut = CreateSut();

            var location = CreateLocation("Stockholm");

            _repo.Setup(x => x.GetByIdAsync(It.IsAny<LocationId>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(location);

            var result = await sut.GetLocationByIdAsync(Guid.NewGuid(), CancellationToken.None);

            result.LocationName.Should().Be("Stockholm");
            result.Id.Should().Be(location.Id.Value);
        }

        // -------------------------
        // SearchLocationsAsync
        // -------------------------

        [Fact]
        public async Task SearchLocationsAsync_ShouldReturnMappedDtos()
        {
            var sut = CreateSut();

            var locations = new List<Location>
            {
                CreateLocation("Stockholm"),
                CreateLocation("Stockholm City")
            };

            _repo.Setup(x => x.SearchByNameAsync("stock", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(locations);

            var result = (await sut.SearchLocationsAsync("stock", CancellationToken.None)).ToList();

            result.Should().HaveCount(2);
            result.All(x => x.LocationName.StartsWith("Stockholm")).Should().BeTrue();
        }

        // -------------------------
        // UpdateLocationAsync
        // -------------------------

        [Fact]
        public async Task UpdateLocationAsync_WhenNotFound_ShouldThrow()
        {
            var sut = CreateSut();

            _repo.Setup(x => x.GetByIdAsync(It.IsAny<LocationId>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync((Location?)null);

            var dto = new UpdateLocationDTO
            {
                LocationName = "New Name",
                RowVersion = new byte[] { 1 }
            };

            var act = async () => await sut.UpdateLocationAsync(Guid.NewGuid(), dto, CancellationToken.None);

            await act.Should().ThrowAsync<LocationNotFoundException>();

            _repo.Verify(x => x.UpdateAsync(It.IsAny<Location>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()), Times.Never);
            _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdateLocationAsync_WhenNameChangedAndExists_ShouldThrow()
        {
            var sut = CreateSut();

            var location = CreateLocation("Stockholm");

            _repo.Setup(x => x.GetByIdAsync(It.IsAny<LocationId>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(location);

            _repo.Setup(x => x.ExistsByNameAsync(It.IsAny<LocationName>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(true);

            var dto = new UpdateLocationDTO
            {
                LocationName = "Göteborg",
                RowVersion = new byte[] { 1 }
            };

            var act = async () => await sut.UpdateLocationAsync(Guid.NewGuid(), dto, CancellationToken.None);

            await act.Should().ThrowAsync<LocationNameAllreadyExistsException>();

            _repo.Verify(x => x.UpdateAsync(It.IsAny<Location>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()), Times.Never);
            _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdateLocationAsync_WhenNameIsSameAfterNormalization_ShouldNotCheckExists()
        {
            var sut = CreateSut();

            var location = CreateLocation("Stockholm City");

            _repo.Setup(x => x.GetByIdAsync(It.IsAny<LocationId>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(location);

            _repo.Setup(x => x.UpdateAsync(It.IsAny<Location>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()))
                 .Returns(Task.CompletedTask);

            _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var dto = new UpdateLocationDTO
            {
                LocationName = "  stockholm   city ",
                RowVersion = new byte[] { 7 }
            };

            var result = await sut.UpdateLocationAsync(Guid.NewGuid(), dto, CancellationToken.None);

            _repo.Verify(x => x.ExistsByNameAsync(It.IsAny<LocationName>(), It.IsAny<CancellationToken>()), Times.Never);
            _repo.Verify(x => x.UpdateAsync(location, dto.RowVersion, It.IsAny<CancellationToken>()), Times.Once);
            _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            result.LocationName.Should().Be("Stockholm City");
        }

        [Fact]
        public async Task UpdateLocationAsync_HappyPath_ShouldUpdate_Save_AndReturnDto()
        {
            var sut = CreateSut();

            var location = CreateLocation("Stockholm");

            _repo.Setup(x => x.GetByIdAsync(It.IsAny<LocationId>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(location);

            _repo.Setup(x => x.ExistsByNameAsync(It.IsAny<LocationName>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(false);

            _repo.Setup(x => x.UpdateAsync(It.IsAny<Location>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()))
                 .Returns(Task.CompletedTask);

            _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var dto = new UpdateLocationDTO
            {
                LocationName = "Göteborg",
                RowVersion = new byte[] { 1, 2 }
            };

            var result = await sut.UpdateLocationAsync(Guid.NewGuid(), dto, CancellationToken.None);

            location.LocationName.Should().Be(LocationName.Create("Göteborg"));

            _repo.Verify(x => x.UpdateAsync(location, dto.RowVersion, It.IsAny<CancellationToken>()), Times.Once);
            _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            result.Id.Should().Be(location.Id.Value);
            result.LocationName.Should().Be(location.LocationName.Value);
        }

        // -------------------------
        // Helpers
        // -------------------------

        private static Location CreateLocation(string name)
            => Location.Create(LocationName.Create(name));
    }
}
