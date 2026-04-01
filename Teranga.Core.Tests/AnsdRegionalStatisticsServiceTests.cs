using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Teranga.Core.Extensions;
using Teranga.Core.Services;

namespace Teranga.Core.Tests
{
    public class AnsdRegionalStatisticsServiceTests
    {
        private static readonly string[] SuspiciousFragments = ["Ã", "�"];
        private readonly AnsdRegionalStatisticsService _service;

        public AnsdRegionalStatisticsServiceTests()
        {
            var logger = new Mock<ILogger<AnsdRegionalStatisticsService>>();
            _service = new AnsdRegionalStatisticsService(logger.Object);
        }

        [Fact]
        public async Task GetMetadataAsync_ShouldReturnNormalizedDatasetMetadata()
        {
            var metadata = await _service.GetMetadataAsync();

            Assert.Equal("Teranga.Core ANSD Regional Profiles", metadata.DatasetName);
            Assert.Contains("ANSD", metadata.SourceOrganization, StringComparison.OrdinalIgnoreCase);
            Assert.Equal("Region", metadata.CoverageLevel);
            Assert.Equal("2026-04-01", metadata.NormalizedOn);
        }

        [Fact]
        public async Task GetIndicatorDefinitionsAsync_ShouldReturnExpectedDefinitions()
        {
            var definitions = await _service.GetIndicatorDefinitionsAsync();

            Assert.Equal(8, definitions.Count);
            Assert.Contains(definitions, definition => definition.IndicatorCode == "population_total");
            Assert.Contains(definitions, definition => definition.IndicatorName == "Taux d'alphabétisation général (%)");
        }

        [Fact]
        public async Task GetAllRegionalProfilesAsync_ShouldCoverAllRegions()
        {
            var profiles = await _service.GetAllRegionalProfilesAsync();

            Assert.Equal(14, profiles.Count);
            Assert.Equal(
                profiles.Count,
                profiles.Select(profile => profile.RegionCode).Distinct(StringComparer.OrdinalIgnoreCase).Count());

            Assert.Contains(profiles, profile => profile.RegionCode == "DK" && profile.RegionName == "Dakar");
            Assert.Contains(profiles, profile => profile.RegionCode == "TH" && profile.RegionName == "Thiès");
            Assert.Contains(profiles, profile => profile.RegionCode == "KE" && profile.RegionName == "Kédougou");

            Assert.All(profiles, profile =>
            {
                Assert.Equal(8, profile.Indicators.Count);
                Assert.Equal(
                    profile.Indicators.Count,
                    profile.Indicators.Select(indicator => indicator.IndicatorCode).Distinct(StringComparer.OrdinalIgnoreCase).Count());
            });
        }

        [Fact]
        public async Task GetRegionalProfileAsync_ForDakar_ShouldReturnExpectedIndicators()
        {
            var profile = await _service.GetRegionalProfileAsync("DK");

            Assert.NotNull(profile);
            Assert.Equal("DK", profile.RegionCode);
            Assert.Equal("Dakar", profile.RegionName);
            Assert.Equal(8, profile.Indicators.Count);
        }

        [Fact]
        public async Task GetRegionalIndicatorAsync_ForDakarPopulation_ShouldReturnExpectedValue()
        {
            var indicator = await _service.GetRegionalIndicatorAsync("DK", "population_total");

            Assert.NotNull(indicator);
            Assert.Equal("population_total", indicator.IndicatorCode);
            Assert.Equal("Population", indicator.IndicatorName);
            Assert.Equal(3896564m, indicator.Value);
            Assert.Equal(2023, indicator.Year);
            Assert.Equal("ANSD/RGPH5-2023", indicator.SourceLabel);
        }

        [Fact]
        public async Task GetRegionalProfileAsync_ForUnknownRegion_ShouldReturnNull()
        {
            var profile = await _service.GetRegionalProfileAsync("XX");

            Assert.Null(profile);
        }

        [Fact]
        public async Task GetRegionalIndicatorAsync_ForUnknownIndicator_ShouldReturnNull()
        {
            var indicator = await _service.GetRegionalIndicatorAsync("DK", "unknown");

            Assert.Null(indicator);
        }

        [Fact]
        public async Task DatasetStrings_ShouldNotContainMojibakeFragments()
        {
            var metadata = await _service.GetMetadataAsync();
            var definitions = await _service.GetIndicatorDefinitionsAsync();
            var profiles = await _service.GetAllRegionalProfilesAsync();

            var values = new List<string>
            {
                metadata.DatasetName,
                metadata.SourceOrganization,
                metadata.SourceCatalogUrl,
                metadata.CoverageLevel,
                metadata.NormalizedOn,
                metadata.Notes
            };

            values.AddRange(definitions.SelectMany(definition => new[]
            {
                definition.IndicatorCode,
                definition.IndicatorName,
                definition.Unit,
                definition.Description ?? string.Empty
            }));

            values.AddRange(profiles.SelectMany(profile => new[]
            {
                profile.RegionCode,
                profile.RegionName,
                profile.ProfilePageUrl
            }));

            values.AddRange(profiles.SelectMany(profile => profile.Indicators.SelectMany(indicator => new[]
            {
                indicator.IndicatorCode,
                indicator.IndicatorName,
                indicator.Unit,
                indicator.SourceLabel,
                indicator.SourceReferenceUrl
            })));

            Assert.All(values, value =>
            {
                Assert.All(SuspiciousFragments, fragment => Assert.DoesNotContain(fragment, value, StringComparison.Ordinal));
            });
        }

        [Fact]
        public void AddTerangaCore_ShouldRegisterAdministrativeAndAnsdServices()
        {
            var services = new ServiceCollection();

            services.AddTerangaCore();

            var terangaDescriptor = Assert.Single(services, service => service.ServiceType == typeof(ITerangaService));
            var ansdDescriptor = Assert.Single(services, service => service.ServiceType == typeof(IAnsdRegionalStatisticsService));

            Assert.Equal(ServiceLifetime.Singleton, terangaDescriptor.Lifetime);
            Assert.Equal(ServiceLifetime.Singleton, ansdDescriptor.Lifetime);
        }
    }
}
