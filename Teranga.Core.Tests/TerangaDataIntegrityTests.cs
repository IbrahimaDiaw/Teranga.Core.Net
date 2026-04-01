using Microsoft.Extensions.Logging;
using Moq;
using Teranga.Core.Models;
using Teranga.Core.Services;

namespace Teranga.Core.Tests
{
    public class TerangaDataIntegrityTests
    {
        private static readonly char[] SuspiciousCharacters = ['Ã', 'Â', '\uFFFD'];
        private readonly TerangaService _service;

        public TerangaDataIntegrityTests()
        {
            var logger = new Mock<ILogger<TerangaService>>();
            var ansdLogger = new Mock<ILogger<AnsdRegionalStatisticsService>>();
            _service = new TerangaService(logger.Object, new AnsdRegionalStatisticsService(ansdLogger.Object));
        }

        [Fact]
        public async Task TerangaData_ShouldNotContainMojibakeCharacters()
        {
            var data = await _service.GetTerangaDataAsync();

            var values = EnumerateStrings(data).ToArray();

            Assert.All(values, value =>
            {
                Assert.DoesNotContain(value, character => SuspiciousCharacters.Contains(character));
            });
        }

        [Fact]
        public async Task TerangaData_ShouldHaveUniqueCodesAndConsistentReferences()
        {
            var data = await _service.GetTerangaDataAsync();
            var regions = data.Regions.ToArray();
            var departements = regions.SelectMany(region => region.Departements).ToArray();
            var communes = departements.SelectMany(departement => departement.Communes).ToArray();

            Assert.Equal(regions.Length, regions.Select(region => region.Code).Distinct(StringComparer.OrdinalIgnoreCase).Count());
            Assert.Equal(departements.Length, departements.Select(departement => departement.Code).Distinct(StringComparer.OrdinalIgnoreCase).Count());
            Assert.Equal(communes.Length, communes.Select(commune => commune.Code).Distinct(StringComparer.OrdinalIgnoreCase).Count());

            Assert.All(regions, region =>
            {
                Assert.False(string.IsNullOrWhiteSpace(region.Code));
                Assert.False(string.IsNullOrWhiteSpace(region.Name));
            });

            Assert.All(departements, departement =>
            {
                Assert.False(string.IsNullOrWhiteSpace(departement.Code));
                Assert.False(string.IsNullOrWhiteSpace(departement.Name));
                Assert.Contains(regions, region => string.Equals(region.Code, departement.RegionCode, StringComparison.OrdinalIgnoreCase));
                Assert.StartsWith(departement.RegionCode, departement.Code, StringComparison.OrdinalIgnoreCase);
            });

            Assert.All(communes, commune =>
            {
                Assert.False(string.IsNullOrWhiteSpace(commune.Code));
                Assert.False(string.IsNullOrWhiteSpace(commune.Name));
                Assert.Contains(departements, departement => string.Equals(departement.Code, commune.DepartementCode, StringComparison.OrdinalIgnoreCase));
                Assert.StartsWith(commune.DepartementCode, commune.Code, StringComparison.OrdinalIgnoreCase);
            });
        }

        [Fact]
        public async Task Communes_ShouldHaveValidCodes_AndBeUnderTheCorrectHierarchy()
        {
            var data = await _service.GetTerangaDataAsync();
            var regionsByCode = data.Regions.ToDictionary(region => region.Code, StringComparer.OrdinalIgnoreCase);
            var departements = data.Regions.SelectMany(region => region.Departements).ToArray();
            var departementsByCode = departements.ToDictionary(departement => departement.Code, StringComparer.OrdinalIgnoreCase);

            Assert.Equal(552, departements.Sum(departement => departement.Communes.Count));

            foreach (var region in data.Regions)
            {
                foreach (var departement in region.Departements)
                {
                    Assert.Matches("^[A-Z]{2}[0-9]+$", departement.Code);
                    Assert.Equal(region.Code, departement.RegionCode, ignoreCase: true);

                    foreach (var commune in departement.Communes)
                    {
                        Assert.Matches("^[A-Z]{2}[0-9]+C[0-9]+$", commune.Code);
                        Assert.False(string.IsNullOrWhiteSpace(commune.Name));
                        Assert.Equal(departement.Code, commune.DepartementCode, ignoreCase: true);
                        Assert.StartsWith(departement.Code, commune.Code, StringComparison.OrdinalIgnoreCase);

                        var parentDepartement = Assert.Single(
                            departementsByCode,
                            item => string.Equals(item.Key, commune.DepartementCode, StringComparison.OrdinalIgnoreCase)).Value;

                        Assert.Equal(region.Code, parentDepartement.RegionCode, ignoreCase: true);
                        Assert.True(
                            regionsByCode.ContainsKey(parentDepartement.RegionCode),
                            $"Unknown parent region for commune {commune.Code}");
                    }
                }
            }
        }

        private static IEnumerable<string> EnumerateStrings(TerangaData data)
        {
            yield return data.Code;
            yield return data.Name;
            yield return data.Description ?? string.Empty;
            yield return data.Capital;
            yield return data.Currency;
            yield return data.Language;
            yield return data.Continent;
            yield return data.TimeZone;
            yield return data.Flag;

            foreach (var localLanguage in data.LocalLanguages)
            {
                yield return localLanguage;
            }

            foreach (var region in data.Regions)
            {
                foreach (var value in EnumerateStrings(region))
                {
                    yield return value;
                }
            }
        }

        private static IEnumerable<string> EnumerateStrings(Region region)
        {
            yield return region.Code;
            yield return region.Name;
            yield return region.Description ?? string.Empty;

            foreach (var departement in region.Departements)
            {
                foreach (var value in EnumerateStrings(departement))
                {
                    yield return value;
                }
            }
        }

        private static IEnumerable<string> EnumerateStrings(Departement departement)
        {
            yield return departement.Code;
            yield return departement.Name;
            yield return departement.Description ?? string.Empty;
            yield return departement.RegionCode;

            foreach (var commune in departement.Communes)
            {
                foreach (var value in EnumerateStrings(commune))
                {
                    yield return value;
                }
            }
        }

        private static IEnumerable<string> EnumerateStrings(Commune commune)
        {
            yield return commune.Code;
            yield return commune.Name;
            yield return commune.Description ?? string.Empty;
            yield return commune.DepartementCode;
        }
    }
}
