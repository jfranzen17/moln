using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DocumentEntities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
  public class ValuationRepository : IValuationRepository
  {
    private readonly CosmosContext _context;
    public ValuationRepository(CosmosContext context)
    {
      _context = context;
    }

    public async Task AddValuationAsync(Valuation valuation)
    {
      valuation.Status = "New";
      valuation.ValuationType = "Normal";
      valuation.Value = 0;

      await _context.AddAsync(valuation);
    }

    public async Task<Valuation> GetValuationAsync(string id)
    {
      return await _context.Valuations.FindAsync(id);
    }

    public async Task<IList<Valuation>> GetValuationByManufacturerAsync(string make)
    {
      return await _context.Valuations.Where(c => c.Vehicle.Make == make).ToListAsync();
    }

    public async Task<Valuation> GetValuationByRegNumber(string regNo)
    {
      return await _context.Valuations.FirstOrDefaultAsync(c => c.Vehicle.RegistrationNo == regNo);
    }

    public async Task<IList<Valuation>> GetValuationByStatus(string status)
    {
      return await _context.Valuations.Where(c => c.Status == status).ToListAsync();
    }

    public async Task<IList<Valuation>> GetValuationsAsync()
    {
      return await _context.Valuations.ToListAsync();
    }

    public void RemoveValuation(Valuation valuation)
    {
      _context.Valuations.Remove(valuation);
    }

    public async Task<bool> SaveChangesAsync()
    {
      return await _context.SaveChangesAsync() > 0;
    }

    public void UpdateValuation(Valuation valuation)
    {
      _context.Update(valuation);
    }
  }
}