# Teranga.Core

[![NuGet](https://img.shields.io/nuget/v/Teranga.Core.svg)](https://www.nuget.org/packages/Teranga.Core/)
[![NuGet](https://img.shields.io/nuget/dt/Teranga.Core.svg)](https://www.nuget.org/packages/Teranga.Core/)

Teranga.Core is a .NET library providing easy access to Senegal's administrative data (regions, departments, and communes).

## Features

- ✨ Complete administrative data of Senegal
- 🚀 High performance and thread-safe
- 🔄 Asynchronous operations
- 🎯 Zero configuration required
- 📦 Embedded data
- 🧪 Fully tested

## Installation

### Via Package Manager Console
```powershell
Install-Package Teranga.Core
```

### Via .NET CLI
```bash
dotnet add package Teranga.Core
```

## Quick Start

1. Register the service in your application:
```csharp
using Teranga.Core.Extensions;

services.AddTerangaCore();
```

2. Use it in your application:
```csharp
public class RegionsController : ControllerBase
{
    private readonly ITerangaService _terangaService;

    public RegionsController(ITerangaService terangaService)
    {
        _terangaService = terangaService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRegions()
    {
        var regions = await _terangaService.GetAllRegionsAsync();
        return Ok(regions);
    }
}
```

## Usage Examples

### Getting All Regions
```csharp
var regions = await _terangaService.GetAllRegionsAsync();
```

### Finding a Specific Region
```csharp
var dakar = await _terangaService.GetRegionByCodeAsync("DK");
```

### Getting Departments in a Region
```csharp
var departments = await _terangaService.GetDepartmentsByRegionAsync("DK");
```

### Finding a Specific Commune
```csharp
var commune = await _terangaService.GetCommuneByCodeAsync("DK1C1");
```

## Administrative Codes

### Structure
- Regions: 2 characters (e.g., "DK" for Dakar)
- Departments: 3 characters (e.g., "DK1")
- Communes: 5 characters (e.g., "DK1C1")

### Examples
```plaintext
DK   -> Dakar (Region)
DK1  -> Dakar (Department)
DK1C17-> Plateau (Commune)
```

## API Reference

### ITerangaService
```csharp
public interface ITerangaService
{
    Task<TerangaData> GetTerangaDataAsync();
    Task<IEnumerable<Region>> GetAllRegionsAsync();
    Task<Region?> GetRegionByCodeAsync(string code);
    Task<IEnumerable<Department>> GetDepartmentsByRegionAsync(string regionCode);
    Task<Department?> GetDepartmentByCodeAsync(string code);
    Task<IEnumerable<Commune>> GetCommunesByDepartmentAsync(string departmentCode);
    Task<Commune?> GetCommuneByCodeAsync(string code);
}
```

## Performance

- Initial load time: < 100ms
- Simple queries: < 10ms
- Complex queries: < 50ms
- Memory usage: < 10MB

## Best Practices

### Do's
```csharp
// Use dependency injection
services.AddTerangaCore();

// Handle null results
var region = await service.GetRegionByCodeAsync(code);
if (region == null) return NotFound();

// Use async/await
await service.GetAllRegionsAsync();
```

### Don'ts
```csharp
// Don't create instances manually
var service = new TerangaService(); // ❌

// Don't ignore null checks
return Ok(await service.GetRegionByCodeAsync(code)); // ❌

// Don't block on async calls
service.GetAllRegionsAsync().Result; // ❌
```

## Error Handling

```csharp
try
{
    var region = await _service.GetRegionByCodeAsync(code);
    if (region == null)
    {
        // Handle not found case
        return NotFound();
    }
    return Ok(region);
}
catch (TerangaException ex)
{
    // Handle specific exceptions
    _logger.LogError(ex, "Error retrieving region");
    return StatusCode(500, "An error occurred");
}
```

## Logging

The service uses Microsoft.Extensions.Logging:
```csharp
services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.AddDebug();
});
```

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## Running Tests

```bash
dotnet test
```

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

- 📫 Report issues on GitHub
- 🌟 Star the repo if you find it helpful
- 🤝 Contribute via pull requests

## Acknowledgments

- Data provided by official Senegalese administrative sources
- Built with .NET 8.0
- Inspired by the need for standardized administrative data access


## 📞 Contact

- Email : ibrahimadiaw1997@gmail.com
- X : [@IbrahimaDiaw](https://x.com/IbrahimaIbnOmar)
- GitHub : [@IbrahimaDiaw](https://github.com/IbrahimaDiaw)
- LinkedIn : [@ibrahimaDiaw](https://www.linkedin.com/in/ibrahima-diaw-0540a71b9/)

---

Made with ❤️ for Senegal