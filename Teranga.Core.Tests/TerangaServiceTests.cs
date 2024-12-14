using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using Teranga.Core.Exceptions;
using Teranga.Core.Models;
using Teranga.Core.Services;

namespace Teranga.Core.Tests
{
    public class TerangaServiceTests
    {
        private readonly Mock<ILogger<TerangaService>> _loggerMock;
        private readonly TerangaService _service;
        private readonly TerangaData _testData;

        public TerangaServiceTests()
        {
            _loggerMock = new Mock<ILogger<TerangaService>>();
            _testData = CreateTestData();
            SetupTestEnvironment();
            _service = new TerangaService(_loggerMock.Object);
        }

        private void SetupTestEnvironment()
        {
            var jsonData = JsonSerializer.Serialize(_testData);
            var directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            Directory.CreateDirectory(directory);
            File.WriteAllText(Path.Combine(directory, "teranga-data.json"), jsonData);
        }

        [Fact]
        public async Task GetTerangaDataAsync_ShouldReturnValidData()
        {
            // Act
            var result = await _service.GetTerangaDataAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("SN", result.Code);
            Assert.Equal("Sénégal", result.Name);
            Assert.Equal(16743930, result.Population);
        }
        [Fact]
        public async Task GetAllRegionsAsync_ShouldReturnAllRegions()
        {
            // Act
            var regions = await _service.GetAllRegionsAsync();

            // Assert
            Assert.NotNull(regions);
            Assert.Single(regions);
            Assert.Equal("DK", regions.First().Code);
        }

        [Theory]
        [InlineData("DK")]
        public async Task GetRegionByCodeAsync_WithValidCode_ShouldReturnRegion(string code)
        {
            // Act
            var region = await _service.GetRegionByCodeAsync(code);

            // Assert
            Assert.NotNull(region);
            Assert.Equal(code, region.Code);
            Assert.Equal("Dakar", region.Name);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData(" ")]
        public async Task GetRegionByCodeAsync_WithInvalidCode_ShouldThrowArgumentNullException(string code)
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _service.GetRegionByCodeAsync(code));
        }

        [Theory]
        [InlineData("DK1")]
        public async Task GetDepartmentByCodeAsync_WithValidCode_ShouldReturnDepartment(string code)
        {
            // Act
            var department = await _service.GetDepartmentByCodeAsync(code);

            // Assert
            Assert.NotNull(department);
            Assert.Equal(code, department.Code);
            Assert.Equal("Dakar", department.Name);
        }

        [Theory]
        [InlineData("DK")]
        public async Task GetDepartmentsByRegionAsync_WithValidRegionCode_ShouldReturnDepartments(string regionCode)
        {
            // Act
            var departments = await _service.GetDepartmentsByRegionAsync(regionCode);

            // Assert
            Assert.NotNull(departments);
            Assert.Single(departments);
            Assert.Equal("DK1", departments.First().Code);
        }

        [Theory]
        [InlineData("DK1C1")]
        public async Task GetCommuneByCodeAsync_WithValidCode_ShouldReturnCommune(string code)
        {
            // Act
            var commune = await _service.GetCommuneByCodeAsync(code);

            // Assert
            Assert.NotNull(commune);
            Assert.Equal(code, commune.Code);
            Assert.Equal("Plateau", commune.Name);
        }

        [Theory]
        [InlineData("DK1")]
        public async Task GetCommunesByDepartmentAsync_WithValidDepartmentCode_ShouldReturnCommunes(string departmentCode)
        {
            // Act
            var communes = await _service.GetCommunesByDepartmentAsync(departmentCode);

            // Assert
            Assert.NotNull(communes);
            Assert.Single(communes);
            Assert.Equal("DK1C1", communes.First().Code);
        }

        [Fact]
        public async Task ReloadDataAsync_ShouldReloadData()
        {
            // Act
            await _service.ReloadDataAsync();

            // Assert
            var data = await _service.GetTerangaDataAsync();
            Assert.NotNull(data);
            Assert.Equal("SN", data.Code);
        }

        [Fact]
        public async Task GetTerangaDataAsync_WithInvalidJson_ShouldThrowTerangaException()
        {
            // Arrange
            File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "teranga-data.json"), "invalid json");
            var service = new TerangaService(_loggerMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<TerangaException>(() =>
                service.GetTerangaDataAsync());
        }

        private TerangaData CreateTestData()
        {
            return new TerangaData
            {
                Code = "SN",
                Name = "Sénégal",
                Population = 16743930,
                Area = 196722,
                Capital = "Dakar",
                Currency = "XOF",
                Language = "Français",
                Continent = "Afrique",
                Regions = new List<Region>
                {
                    new Region
                    {
                        Code = "DK",
                        Name = "Dakar",
                        Departments = new List<Department>
                        {
                            new Department
                            {
                                Code = "DK1",
                                Name = "Dakar",
                                RegionCode = "DK",
                                Communes = new List<Commune>
                                {
                                    new Commune
                                    {
                                        Code = "DK1C1",
                                        Name = "Plateau",
                                        DepartmentCode = "DK1",
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}
