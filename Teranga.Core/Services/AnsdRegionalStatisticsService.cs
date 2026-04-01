using System.Text.Json;
using Microsoft.Extensions.Logging;
using Teranga.Core.Exceptions;
using Teranga.Core.Models;

namespace Teranga.Core.Services
{
    /// <summary>
    /// Loads and exposes a normalized regional ANSD dataset embedded in the package.
    /// </summary>
    public sealed class AnsdRegionalStatisticsService : IAnsdRegionalStatisticsService
    {
        private const string ResourceName = "Teranga.Core.Data.ansd-regional-profiles.json";

        private readonly ILogger<AnsdRegionalStatisticsService> _logger;
        private AnsdDatasetMetadata? _metadata;
        private AnsdIndicatorDefinition[] _definitions = [];
        private AnsdRegionalProfile[] _profiles = [];
        private Dictionary<string, AnsdRegionalProfile> _profilesByCode = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Initializes a new instance of the <see cref="AnsdRegionalStatisticsService"/> class.
        /// </summary>
        public AnsdRegionalStatisticsService(ILogger<AnsdRegionalStatisticsService> logger)
        {
            _logger = logger;
            LoadData();
        }

        /// <summary>
        /// Get dataset metadata.
        /// </summary>
        public Task<AnsdDatasetMetadata> GetMetadataAsync()
        {
            _logger.LogInformation("Retrieving ANSD dataset metadata");
            return Task.FromResult(EnsureMetadataLoaded());
        }

        /// <summary>
        /// Get the list of supported indicators.
        /// </summary>
        public Task<IReadOnlyList<AnsdIndicatorDefinition>> GetIndicatorDefinitionsAsync()
        {
            _logger.LogInformation("Retrieving ANSD indicator definitions");
            return Task.FromResult<IReadOnlyList<AnsdIndicatorDefinition>>(_definitions);
        }

        /// <summary>
        /// Get all regional profiles.
        /// </summary>
        public Task<IReadOnlyList<AnsdRegionalProfile>> GetAllRegionalProfilesAsync()
        {
            _logger.LogInformation("Retrieving all ANSD regional profiles");
            return Task.FromResult<IReadOnlyList<AnsdRegionalProfile>>(_profiles);
        }

        /// <summary>
        /// Get a regional profile by region code.
        /// </summary>
        public Task<AnsdRegionalProfile?> GetRegionalProfileAsync(string regionCode)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(regionCode);

            _logger.LogInformation("Retrieving ANSD regional profile for: {RegionCode}", regionCode);
            _profilesByCode.TryGetValue(regionCode.Trim(), out var profile);
            return Task.FromResult(profile);
        }

        /// <summary>
        /// Get a single indicator for a region.
        /// </summary>
        public async Task<AnsdRegionalIndicator?> GetRegionalIndicatorAsync(string regionCode, string indicatorCode)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(regionCode);
            ArgumentException.ThrowIfNullOrWhiteSpace(indicatorCode);

            var profile = await GetRegionalProfileAsync(regionCode);
            if (profile is null)
            {
                return null;
            }

            _logger.LogInformation(
                "Retrieving ANSD regional indicator {IndicatorCode} for: {RegionCode}",
                indicatorCode,
                regionCode);

            return profile.Indicators.FirstOrDefault(
                indicator => string.Equals(indicator.IndicatorCode, indicatorCode.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        private void LoadData()
        {
            try
            {
                var assembly = typeof(AnsdRegionalStatisticsService).Assembly;
                using var stream = assembly.GetManifestResourceStream(ResourceName);
                if (stream is null)
                {
                    throw new TerangaException("Resource not found: " + ResourceName);
                }

                using var reader = new StreamReader(stream);
                var jsonContent = reader.ReadToEnd();

                var document = JsonSerializer.Deserialize<AnsdRegionalDatasetDocument>(
                    jsonContent,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (document is null || document.Metadata is null)
                {
                    throw new TerangaException("Failed to deserialize ANSD regional dataset");
                }

                _metadata = document.Metadata;
                _definitions = document.IndicatorDefinitions.ToArray();

                var definitionsByCode = _definitions.ToDictionary(
                    definition => definition.IndicatorCode,
                    StringComparer.OrdinalIgnoreCase);

                _profiles = document.Profiles
                    .Select(profile => new AnsdRegionalProfile
                    {
                        RegionCode = profile.RegionCode,
                        RegionName = profile.RegionName,
                        ProfilePageUrl = profile.ProfilePageUrl,
                        Indicators = profile.Indicators.Select(indicator =>
                        {
                            if (!definitionsByCode.TryGetValue(indicator.IndicatorCode, out var definition))
                            {
                                throw new TerangaException(
                                    $"Missing indicator definition for code: {indicator.IndicatorCode}");
                            }

                            return new AnsdRegionalIndicator
                            {
                                IndicatorCode = indicator.IndicatorCode,
                                IndicatorName = definition.IndicatorName,
                                Value = indicator.Value,
                                Unit = definition.Unit,
                                Year = indicator.Year,
                                SourceLabel = indicator.SourceLabel,
                                SourceReferenceUrl = indicator.SourceReferenceUrl
                            };
                        }).ToArray()
                    })
                    .OrderBy(profile => profile.RegionName, StringComparer.OrdinalIgnoreCase)
                    .ToArray();

                _profilesByCode = _profiles.ToDictionary(profile => profile.RegionCode, StringComparer.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading ANSD regional dataset");
                throw new TerangaException("Failed to load ANSD regional dataset", ex);
            }
        }

        private AnsdDatasetMetadata EnsureMetadataLoaded()
        {
            return _metadata ?? throw new TerangaException("ANSD regional dataset metadata not loaded");
        }

        private sealed class AnsdRegionalDatasetDocument
        {
            public AnsdDatasetMetadata? Metadata { get; init; }

            public List<AnsdIndicatorDefinition> IndicatorDefinitions { get; init; } = [];

            public List<AnsdRegionalProfileDocument> Profiles { get; init; } = [];
        }

        private sealed class AnsdRegionalProfileDocument
        {
            public string RegionCode { get; init; } = default!;

            public string RegionName { get; init; } = default!;

            public string ProfilePageUrl { get; init; } = default!;

            public List<AnsdRegionalIndicatorDocument> Indicators { get; init; } = [];
        }

        private sealed class AnsdRegionalIndicatorDocument
        {
            public string IndicatorCode { get; init; } = default!;

            public decimal Value { get; init; }

            public int? Year { get; init; }

            public string SourceLabel { get; init; } = default!;

            public string SourceReferenceUrl { get; init; } = default!;
        }
    }
}
