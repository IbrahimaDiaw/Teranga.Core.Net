using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teranga.Core.Models;

namespace Teranga.Core.Services
{
    public interface ITerangaService
    {
        Task<TerangaData> GetTerangaDataAsync();
        Task<IEnumerable<Region>> GetAllRegionsAsync();
        Task<Region?> GetRegionByCodeAsync(string code);
        Task<IEnumerable<Department>> GetDepartmentsByRegionAsync(string regionCode);
        Task<Department?> GetDepartmentByCodeAsync(string code);
        Task<IEnumerable<Commune>> GetCommunesByDepartmentAsync(string departmentCode);
        Task<Commune?> GetCommuneByCodeAsync(string code);
        Task ReloadDataAsync();
    }
}
