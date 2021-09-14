using System.Collections.Generic;
using System.Threading.Tasks;
using API.DocumentEntities;

namespace API.Interfaces
{
  public interface IValuationRepository
  {
    Task<IList<Valuation>> GetValuationsAsync();
    Task<Valuation> GetValuationAsync(string id);
    Task<IList<Valuation>> GetValuationByManufacturerAsync(string make);
    Task<Valuation> GetValuationByRegNumber(string regNo);
    Task<IList<Valuation>> GetValuationByStatus(string status);
    void RemoveValuation(Valuation valuation);
    Task AddValuationAsync(Valuation valuation);
    void UpdateValuation(Valuation valuation);
    Task<bool> SaveChangesAsync();
  }
}