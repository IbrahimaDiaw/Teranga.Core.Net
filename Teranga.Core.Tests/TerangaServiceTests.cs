using Microsoft.Extensions.Logging;
using Moq;
using Teranga.Core.Services;

namespace Teranga.Core.Tests
{
    public class TerangaServiceTests
    {
        private readonly Mock<ILogger<TerangaService>> _logger;
        private readonly TerangaService _service;

        public TerangaServiceTests()
        {
            _logger = new Mock<ILogger<TerangaService>>();
            _service = new TerangaService(_logger.Object);
        }

        [Fact]
        public async Task GetTerangaDataAsync_ShouldReturnData()
        {
            // Act
            var result = await _service.GetTerangaDataAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("SN", result.Code);
            Assert.Equal("Sénégal", result.Name);
        }

        [Fact]
        public async Task GetAllRegionsAsync_ShouldReturnAllRegions()
        {
            // Act
            var regions = await _service.GetAllRegionsAsync();

            // Assert
            Assert.NotNull(regions);
            Assert.NotEmpty(regions);
        }

        [Fact]
        public async Task GetRegionByCodeAsync_WithDakarCode_ShouldReturnDakarRegion()
        {
            // Act
            var region = await _service.GetRegionByCodeAsync("DK");

            // Assert
            Assert.NotNull(region);
            Assert.Equal("DK", region.Code);
            Assert.Equal("Dakar", region.Name);
        }

        [Theory]
        [InlineData("INVALID")]
        public async Task GetRegionByCodeAsync_WithInvalidCode_ShouldReturnNull(string code)
        {
            // Act
            var region = await _service.GetRegionByCodeAsync(code);

            // Assert
            Assert.Null(region);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task GetRegionByCodeAsync_WithInvalidInput_ShouldThrowArgumentException(string code)
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _service.GetRegionByCodeAsync(code));
        }

        [Fact]
        public async Task GetDepartmentsByRegionAsync_ForDakar_ShouldReturnDepartments()
        {
            // Act
            var departments = await _service.GetDepartmentsByRegionAsync("DK");

            // Assert
            Assert.NotNull(departments);
            Assert.NotEmpty(departments);
            Assert.Contains(departments, d => d.Name == "Dakar");
        }

        [Fact]
        public async Task GetDepartmentByCodeAsync_ForDakarDepartment_ShouldReturnDepartment()
        {
            // Act
            var department = await _service.GetDepartmentByCodeAsync("DK1");

            // Assert
            Assert.NotNull(department);
            Assert.Equal("DK1", department.Code);
            Assert.Equal("Dakar", department.Name);
        }

        [Fact]
        public async Task GetCommunesByDepartmentAsync_ForDakarDepartment_ShouldReturnCommunes()
        {
            // Act
            var communes = await _service.GetCommunesByDepartmentAsync("DK1");

            // Assert
            Assert.NotNull(communes);
            Assert.NotEmpty(communes);
        }

        [Fact]
        public async Task GetCommuneByCodeAsync_ForPlateauCommune_ShouldReturnCommune()
        {
            // Act
            var commune = await _service.GetCommuneByCodeAsync("DK1C17");

            // Assert
            Assert.NotNull(commune);
            Assert.Equal("DK1C17", commune.Code);
            Assert.Equal("Plateau", commune.Name);
        }
    }
}
