using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teranga.Core.Models;

namespace Teranga.Core.Services
{
    /// <summary>
    /// The Teranga service
    /// </summary>
    public interface ITerangaService
    {
        /// <summary>
        /// Get the Teranga data
        /// <returns></returns>
        /// </summary>
        Task<TerangaData> GetTerangaDataAsync();
        /// <summary>
        /// Get all the regions
        /// <returns></returns>
        /// </summary>
        Task<IEnumerable<Region>> GetAllRegionsAsync();
        /// <summary>
        /// Get the region by code
        /// <param name="code"></param>
        /// <returns></returns>
        /// </summary>
        Task<Region?> GetRegionByCodeAsync(string code);
        /// <summary>
        /// Get the departments by region
        /// <param name="regionCode"></param>
        /// <returns></returns>
        /// </summary>
        Task<IEnumerable<Departement>> GetDepartmentsByRegionAsync(string regionCode);
        /// <summary>
        /// Get the department by code
        /// <param name="code"></param>
        /// <returns></returns>
        /// </summary>
        Task<Departement?> GetDepartmentByCodeAsync(string code);
        /// <summary>
        /// Get the communes by department
        /// <param name="departmentCode"></param>
        /// <returns></returns>
        /// </summary>
        Task<IEnumerable<Commune>> GetCommunesByDepartmentAsync(string departmentCode);
        /// <summary>
        /// Get the commune by code
        /// <param name="code"></param>
        /// <returns></returns>
        /// </summary>
        Task<Commune?> GetCommuneByCodeAsync(string code);
        /// <summary>
        /// Reload the data
        /// <returns></returns>
        /// </summary>
        Task ReloadDataAsync();
    }
}
