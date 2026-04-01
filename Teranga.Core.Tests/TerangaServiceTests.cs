using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Teranga.Core.Extensions;
using Teranga.Core.Services;

namespace Teranga.Core.Tests
{
    public class TerangaServiceTests
    {
        private readonly Mock<ILogger<TerangaService>> _logger;
        private readonly Mock<ILogger<AnsdRegionalStatisticsService>> _ansdLogger;
        private readonly TerangaService _service;

        public TerangaServiceTests()
        {
            _logger = new Mock<ILogger<TerangaService>>();
            _ansdLogger = new Mock<ILogger<AnsdRegionalStatisticsService>>();
            _service = new TerangaService(_logger.Object, new AnsdRegionalStatisticsService(_ansdLogger.Object));
        }

        [Fact]
        public async Task GetTerangaDataAsync_ShouldReturnData()
        {
            var result = await _service.GetTerangaDataAsync();

            Assert.NotNull(result);
            Assert.Equal("SN", result.Code);
            Assert.Equal("S\u00E9n\u00E9gal", result.Name);
        }

        [Fact]
        public async Task GetAllRegionsAsync_ShouldReturnAllRegions()
        {
            var regions = await _service.GetAllRegionsAsync();

            Assert.NotNull(regions);
            Assert.NotEmpty(regions);
        }

        [Fact]
        public async Task GetRegionByCodeAsync_WithDakarCode_ShouldReturnDakarRegion()
        {
            var region = await _service.GetRegionByCodeAsync("DK");

            Assert.NotNull(region);
            Assert.Equal("DK", region.Code);
            Assert.Equal("Dakar", region.Name);
        }

        [Theory]
        [InlineData("INVALID")]
        public async Task GetRegionByCodeAsync_WithInvalidCode_ShouldReturnNull(string code)
        {
            var region = await _service.GetRegionByCodeAsync(code);

            Assert.Null(region);
        }

        [Fact]
        public async Task GetRegionByCodeAsync_WithNullInput_ShouldThrowArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.GetRegionByCodeAsync(null!));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public async Task GetRegionByCodeAsync_WithBlankInput_ShouldThrowArgumentException(string code)
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _service.GetRegionByCodeAsync(code));
        }

        [Fact]
        public async Task GetDepartmentsByRegionAsync_ForDakar_ShouldReturnDepartments()
        {
            var departments = await _service.GetDepartmentsByRegionAsync("DK");

            Assert.NotNull(departments);
            Assert.NotEmpty(departments);
            Assert.Contains(departments, department => department.Name == "Dakar");
        }

        [Fact]
        public async Task GetDepartmentByCodeAsync_ForDakarDepartment_ShouldReturnDepartment()
        {
            var department = await _service.GetDepartmentByCodeAsync("DK1");

            Assert.NotNull(department);
            Assert.Equal("DK1", department.Code);
            Assert.Equal("Dakar", department.Name);
        }

        [Fact]
        public async Task GetCommunesByDepartmentAsync_ForDakarDepartment_ShouldReturnCommunes()
        {
            var communes = await _service.GetCommunesByDepartmentAsync("DK1");

            Assert.NotNull(communes);
            Assert.NotEmpty(communes);
        }

        [Fact]
        public async Task GetCommuneByCodeAsync_ForPlateauCommune_ShouldReturnCommune()
        {
            var commune = await _service.GetCommuneByCodeAsync("DK1C17");

            Assert.NotNull(commune);
            Assert.Equal("DK1C17", commune.Code);
            Assert.Equal("Plateau", commune.Name);
        }

        [Fact]
        public async Task SearchRegionsByNameAsync_ShouldMatchIgnoringCase()
        {
            var regions = await _service.SearchRegionsByNameAsync("dakar");

            var region = Assert.Single(regions);
            Assert.Equal("DK", region.Code);
        }

        [Fact]
        public async Task SearchCommunesByNameAsync_ShouldMatchIgnoringAccents()
        {
            var communes = await _service.SearchCommunesByNameAsync("medina");

            Assert.Contains(communes, commune => commune.Name.Contains("M\u00E9dina", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task GetStatisticsAsync_ShouldReturnExpectedAggregates()
        {
            var statistics = await _service.GetStatisticsAsync();

            Assert.Equal("SN", statistics.CountryCode);
            Assert.Equal("S\u00E9n\u00E9gal", statistics.CountryName);
            Assert.Equal(14, statistics.RegionCount);
            Assert.Equal(46, statistics.DepartmentCount);
            Assert.Equal(552, statistics.CommuneCount);
            Assert.Equal(17, statistics.LocalLanguageCount);
            Assert.Equal("DK", statistics.RegionWithMostDepartments?.RegionCode);
            Assert.Equal(5, statistics.RegionWithMostDepartments?.DepartmentCount);
            Assert.Equal("LG", statistics.RegionWithMostCommunes?.RegionCode);
            Assert.Equal(55, statistics.RegionWithMostCommunes?.CommuneCount);
            Assert.Equal("SL3", statistics.DepartmentWithMostCommunes?.DepartmentCode);
            Assert.Equal(23, statistics.DepartmentWithMostCommunes?.CommuneCount);
            Assert.Equal(14, statistics.Regions.Count);
            Assert.Equal(10, statistics.TopDepartmentsByCommuneCount.Count);
            Assert.Equal("SL3", statistics.TopDepartmentsByCommuneCount[0].DepartmentCode);
        }

        [Fact]
        public async Task GetRegionStatisticsAsync_ForDakar_ShouldReturnDetailedBreakdown()
        {
            var statistics = await _service.GetRegionStatisticsAsync("DK");

            Assert.NotNull(statistics);
            Assert.Equal("DK", statistics.RegionCode);
            Assert.Equal("Dakar", statistics.RegionName);
            Assert.Equal(5, statistics.DepartmentCount);
            Assert.Equal(53, statistics.CommuneCount);
            Assert.Equal("DK1", statistics.DepartmentWithMostCommunes?.DepartmentCode);
            Assert.Equal(19, statistics.DepartmentWithMostCommunes?.CommuneCount);
            Assert.Equal(5, statistics.Departments.Count);
        }

        [Fact]
        public async Task GetRegionStatisticsAsync_ForUnknownRegion_ShouldReturnNull()
        {
            var statistics = await _service.GetRegionStatisticsAsync("XX");

            Assert.Null(statistics);
        }

        [Fact]
        public async Task GetAnsdRegionalProfileAsync_ForDakar_ShouldReturnExpectedIndicators()
        {
            var profile = await _service.GetAnsdRegionalProfileAsync("DK");

            Assert.NotNull(profile);
            Assert.Equal("DK", profile.RegionCode);
            Assert.Equal(8, profile.Indicators.Count);
        }

        [Fact]
        public async Task GetAnsdMetadataAsync_ShouldExposeAnsdDatasetMetadata()
        {
            var metadata = await _service.GetAnsdMetadataAsync();

            Assert.NotNull(metadata);
            Assert.Contains("ANSD", metadata.SourceOrganization, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task GetRegionProfileAsync_ForDakar_ShouldReturnAdministrativeAndAnsdData()
        {
            var profile = await _service.GetRegionProfileAsync("DK");

            Assert.NotNull(profile);
            Assert.Equal("DK", profile.RegionCode);
            Assert.Equal("Dakar", profile.RegionName);
            Assert.Equal("DK", profile.Region.Code);
            Assert.Equal(5, profile.Departments.Count);
            Assert.Equal(53, profile.Communes.Count);
            Assert.NotNull(profile.Statistics);
            Assert.Equal(53, profile.Statistics.CommuneCount);
            Assert.NotNull(profile.AnsdProfile);
            Assert.Equal(8, profile.AnsdProfile!.Indicators.Count);
        }

        [Fact]
        public async Task GetRegionProfileAsync_ForUnknownRegion_ShouldReturnNull()
        {
            var profile = await _service.GetRegionProfileAsync("XX");

            Assert.Null(profile);
        }

        [Fact]
        public async Task SearchRegionProfilesAsync_ShouldMatchByRegionCodeOrNameIgnoringAccents()
        {
            var dakarMatches = await _service.SearchRegionProfilesAsync("DK");
            var thiesMatches = await _service.SearchRegionProfilesAsync("thies");

            Assert.Contains(dakarMatches, profile => profile.RegionCode == "DK");
            Assert.Contains(thiesMatches, profile => profile.RegionCode == "TH");
        }

        [Fact]
        public async Task SearchRegionProfilesAsync_ForUnknownQuery_ShouldReturnEmptyResults()
        {
            var matches = await _service.SearchRegionProfilesAsync("unknown");

            Assert.Empty(matches);
        }

        [Fact]
        public async Task GetRegionProfilesAsync_ShouldReturnPagedProfiles()
        {
            var page = await _service.GetRegionProfilesAsync(1, 5);

            Assert.Equal(1, page.Page);
            Assert.Equal(5, page.PageSize);
            Assert.Equal(14, page.TotalCount);
            Assert.Equal(3, page.TotalPages);
            Assert.False(page.HasPreviousPage);
            Assert.True(page.HasNextPage);
            Assert.Equal(5, page.Items.Count);
        }

        [Fact]
        public async Task GetRegionProfilesAsync_WithInvalidPage_ShouldThrowArgumentOutOfRangeException()
        {
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _service.GetRegionProfilesAsync(0, 5));
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _service.GetRegionProfilesAsync(1, 0));
        }

        [Fact]
        public void AddTerangaCore_ShouldRegisterSingletonByDefault()
        {
            var services = new ServiceCollection();

            services.AddTerangaCore();

            var descriptor = Assert.Single(services, service => service.ServiceType == typeof(ITerangaService));
            Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
        }
    }
}
