using Microsoft.Extensions.Logging;
using System.Text.Json;
using Teranga.Core.Exceptions;
using Teranga.Core.Models;

namespace Teranga.Core.Services
{
    /// <summary>
    /// Service to retrieve Teranga data
    /// </summary>
    public class TerangaService : ITerangaService
    {
        private readonly ILogger<TerangaService> _logger;
        private TerangaData? _terangaData;
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        /// <summary>
        ///  Initializes a new instance of the <see cref="TerangaService"/> class.
        ///  <param name="logger"></param>
        /// </summary>
        public TerangaService(ILogger<TerangaService> logger)
        {
            _logger = logger;
            LoadInitialData().Wait();
        }

        /// <summary>
        /// Get the Teranga data
        /// <returns></returns>
        /// <exception cref="TerangaException"></exception>
        /// </summary>
        public async Task<TerangaData> GetTerangaDataAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                _logger.LogInformation("Retrieving Teranga data");
                if (_terangaData == null)
                {
                    throw new TerangaException("Teranga data not loaded");
                }
                return _terangaData;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Get all the regions
        /// <returns></returns>
        /// </summary>
        public async Task<IEnumerable<Region>> GetAllRegionsAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                _logger.LogInformation("Retrieving all regions");
                return _terangaData?.Regions.ToList() ?? Enumerable.Empty<Region>();
            }
            finally
            {
                _semaphore.Release();
            }
        }
        /// <summary>
        /// Get the region by code
        /// <param name="code"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// </summary>
        public async Task<Region?> GetRegionByCodeAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentNullException(nameof(code));

            await _semaphore.WaitAsync();
            try
            {
                _logger.LogInformation("Retrieving region with code: {Code}", code);
                return _terangaData?.Regions
                    .FirstOrDefault(r => r.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
            }
            finally
            {
                _semaphore.Release();
            }
        }
        /// <summary>
        /// Get the departments by region
        /// <param name="regionCode"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// </summary>
        public async Task<IEnumerable<Departement>> GetDepartmentsByRegionAsync(string regionCode)
        {
            if (string.IsNullOrWhiteSpace(regionCode))
                throw new ArgumentNullException(nameof(regionCode));

            await _semaphore.WaitAsync();
            try
            {
                _logger.LogInformation("Retrieving departments for region: {RegionCode}", regionCode);
                var region = _terangaData?.Regions
                    .FirstOrDefault(r => r.Code.Equals(regionCode, StringComparison.OrdinalIgnoreCase));
                return region?.Departments ?? Enumerable.Empty<Departement>();
            }
            finally
            {
                _semaphore.Release();
            }
        }
        /// <summary>
        /// Get the department by code
        /// <param name="code"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// </summary>
        public async Task<Departement?> GetDepartmentByCodeAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentNullException(nameof(code));

            await _semaphore.WaitAsync();
            try
            {
                _logger.LogInformation("Retrieving department with code: {Code}", code);
                return _terangaData?.Regions
                    .SelectMany(r => r.Departments)
                    .FirstOrDefault(d => d.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
            }
            finally
            {
                _semaphore.Release();
            }
        }
        /// <summary>
        /// Get the communes by department
        /// <param name="departmentCode"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// </summary>
        public async Task<IEnumerable<Commune>> GetCommunesByDepartmentAsync(string departmentCode)
        {
            if (string.IsNullOrWhiteSpace(departmentCode))
                throw new ArgumentNullException(nameof(departmentCode));

            await _semaphore.WaitAsync();
            try
            {
                _logger.LogInformation("Retrieving communes for department: {DepartmentCode}", departmentCode);
                var department = _terangaData?.Regions
                    .SelectMany(r => r.Departments)
                    .FirstOrDefault(d => d.Code.Equals(departmentCode, StringComparison.OrdinalIgnoreCase));
                return department?.Communes ?? Enumerable.Empty<Commune>();
            }
            finally
            {
                _semaphore.Release();
            }
        }
        /// <summary>
        /// Get the commune by code
        /// <param name="code"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// </summary>
        public async Task<Commune?> GetCommuneByCodeAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentNullException(nameof(code));

            await _semaphore.WaitAsync();
            try
            {
                _logger.LogInformation("Retrieving commune with code: {Code}", code);
                return _terangaData?.Regions
                    .SelectMany(r => r.Departments)
                    .SelectMany(d => d.Communes)
                    .FirstOrDefault(c => c.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
            }
            finally
            {
                _semaphore.Release();
            }
        }
        /// <summary>
        /// Reload the data
        /// <returns></returns>
        /// </summary>
        public async Task ReloadDataAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                await LoadInitialData();
                _logger.LogInformation("Teranga data reloaded successfully");
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task LoadInitialData()
        {
            try
            {
                string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "teranga-data.json");
                string jsonContent = await File.ReadAllTextAsync(jsonPath);
                _terangaData = JsonSerializer.Deserialize<TerangaData>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (_terangaData == null)
                {
                    throw new TerangaException("Failed to deserialize Teranga data");
                }

                _logger.LogInformation("Teranga data loaded successfully. Found {RegionCount} regions", _terangaData.Regions.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Teranga data");
                throw new TerangaException("Failed to load Teranga data", ex);
            }
        }
    }
}
