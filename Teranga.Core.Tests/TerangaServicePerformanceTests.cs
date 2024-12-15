using Microsoft.Extensions.Logging;
using Moq;
using System.Diagnostics;
using Teranga.Core.Services;

namespace Teranga.Core.Tests
{
    public class TerangaServicePerformanceTests
    {
        private readonly Mock<ILogger<TerangaService>> _logger;
        private readonly TerangaService _service;
        private readonly Stopwatch _stopwatch;

        public TerangaServicePerformanceTests()
        {
            _logger = new Mock<ILogger<TerangaService>>();
            _service = new TerangaService(_logger.Object);
            _stopwatch = new Stopwatch();
        }

        [Fact]
        public async Task TerangaService_PerformanceTest_AllOperations()
        {
            // Test GetTerangaDataAsync
            _stopwatch.Start();
            await _service.GetTerangaDataAsync();
            _stopwatch.Stop();
            Assert.True(_stopwatch.ElapsedMilliseconds < 100, "GetTerangaDataAsync took too long");

            // Test GetAllRegionsAsync
            _stopwatch.Restart();
            await _service.GetAllRegionsAsync();
            _stopwatch.Stop();
            Assert.True(_stopwatch.ElapsedMilliseconds < 50, "GetAllRegionsAsync took too long");

            // Test GetRegionByCodeAsync
            _stopwatch.Restart();
            await _service.GetRegionByCodeAsync("DK");
            _stopwatch.Stop();
            Assert.True(_stopwatch.ElapsedMilliseconds < 50, "GetRegionByCodeAsync took too long");

            // Test GetDepartmentsByRegionAsync
            _stopwatch.Restart();
            await _service.GetDepartmentsByRegionAsync("DK");
            _stopwatch.Stop();
            Assert.True(_stopwatch.ElapsedMilliseconds < 50, "GetDepartmentsByRegionAsync took too long");
        }

        [Theory]
        [InlineData(100)] // Nombre d'appels simultanés
        public async Task TerangaService_ConcurrentAccess_Performance(int concurrentCalls)
        {
            // Arrange
            var tasks = new List<Task>();
            _stopwatch.Start();

            // Act
            for (int i = 0; i < concurrentCalls; i++)
            {
                tasks.Add(_service.GetTerangaDataAsync());
                tasks.Add(_service.GetAllRegionsAsync());
                tasks.Add(_service.GetRegionByCodeAsync("DK"));
            }

            await Task.WhenAll(tasks);
            _stopwatch.Stop();

            // Assert
            var averageTimePerOperation = _stopwatch.ElapsedMilliseconds / (concurrentCalls * 3.0);
            Assert.True(averageTimePerOperation < 50,
                $"Average operation time ({averageTimePerOperation}ms) exceeded 50ms threshold");
        }

        [Fact]
        public async Task TerangaService_RepeatedAccess_Performance()
        {
            // Arrange
            const int iterations = 1000;
            var times = new List<long>();

            // Act
            for (int i = 0; i < iterations; i++)
            {
                _stopwatch.Restart();
                await _service.GetAllRegionsAsync();
                _stopwatch.Stop();
                times.Add(_stopwatch.ElapsedMilliseconds);
            }

            // Assert
            var averageTime = times.Average();
            var maxTime = times.Max();

            Assert.True(averageTime < 10, $"Average access time ({averageTime}ms) exceeded 10ms threshold");
            Assert.True(maxTime < 50, $"Maximum access time ({maxTime}ms) exceeded 50ms threshold");
        }

        [Fact]
        public async Task TerangaService_MemoryUsage_Test()
        {
            // Arrange
            var initialMemory = GC.GetTotalMemory(true);
            const int iterations = 1000;

            // Act
            for (int i = 0; i < iterations; i++)
            {
                await _service.GetTerangaDataAsync();
                await _service.GetAllRegionsAsync();
                await _service.GetRegionByCodeAsync("DK");
            }

            GC.Collect();
            var finalMemory = GC.GetTotalMemory(true);
            var memoryPerIteration = (finalMemory - initialMemory) / (double)iterations;

            // Assert
            Assert.True(memoryPerIteration < 1024, // 1KB par itération
                $"Memory usage per iteration ({memoryPerIteration:F2} bytes) exceeded threshold");
        }
    }
}
