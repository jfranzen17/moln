using System;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DocumentEntities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
  [ApiController]
  [Route("api/valuations")]
  public class ValuationsController : ControllerBase
  {
    private readonly IValuationRepository _repo;
    public ValuationsController(IValuationRepository repo)
    {
      _repo = repo;
    }

    public async Task<IActionResult> GetValuations()
    {
      var result = await _repo.GetValuationsAsync();
      return Ok(result);
    }

    //Hämtar alla värderingar baserat på tillverkare av bil
    [HttpGet("bymake/{make}")]
    public async Task<IActionResult> GetValuationByMake(string make)
    {
      var result = await _repo.GetValuationByManufacturerAsync(make);
      if (result == null) return NotFound($"Kunde inte hitta några värderingar för tillverkare {make}");
      return Ok(result);
    }
    //Hämtar värdering baserat på id
    [HttpGet("{id}")]
    public async Task<IActionResult> GetValuationById(string id)
    {
      var result = await _repo.GetValuationAsync(id);
      if (result == null) return NotFound($"Kunde inte hitta någon värdering med id {id}");
      return Ok(result);
    }
    //Hämtar värdering baserat på bilens registreringsnummer.
    [HttpGet("byregno/{regno}")]
    public async Task<IActionResult> GetValuationByRegistrationNumber(string regno)
    {
      var result = await _repo.GetValuationByRegNumber(regno);

      if (result == null) return NotFound($"Kunde inte hitta någon värdering för bil {regno}");
      return Ok(result);
    }

    //Hämtar värdingar baserat på status.
    [HttpGet("bystatus/{status}")]
    public async Task<IActionResult> GetValuationByStatus(string status)
    {
      var result = await _repo.GetValuationByStatus(status);
      if (result == null) return NotFound($"Kunde inte hitta några värderingar med status {status}");
      return Ok(result);
    }

    [HttpPost()]
    public async Task<IActionResult> AddValuation(Valuation valuation)
    {
      try
      {
        var result = await _repo.GetValuationByRegNumber(valuation.Vehicle.RegistrationNo);

        if (result != null) return BadRequest($"En bil med registeringsnummer {valuation.Vehicle.RegistrationNo} har redan en värdering");

        await _repo.AddValuationAsync(valuation);

        if (await _repo.SaveChangesAsync()) return StatusCode(201);
        return StatusCode(500, "Ooops något gick fel när vi skulle spara ner värderingen");
      }
      catch (Exception ex)
      {
        return StatusCode(500, ex.Message);
      }
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateValuation(string id, Valuation model)
    {
      try
      {
        var valuation = await _repo.GetValuationAsync(id);
        if (valuation == null) return NotFound($"Kunde inte hitta någon värdering med id {id}");

        valuation.Status = model.Status;
        valuation.ValuationType = model.ValuationType;
        valuation.Value = model.Value;

        _repo.UpdateValuation(model);

        if (await _repo.SaveChangesAsync()) return NoContent();
        return StatusCode(500, "Ooops något gick fel när värderingen skulle uppdateras.");
      }
      catch (Exception ex)
      {
        return StatusCode(500, ex.Message);
      }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteValuation(string id)
    {
      try
      {
        var valuation = await _repo.GetValuationAsync(id);
        if (valuation == null) return NotFound($"Kunde inte hitta någon värdering med id: {id}");

        _repo.RemoveValuation(valuation);

        if (await _repo.SaveChangesAsync()) return NoContent();
        return StatusCode(500, "Ooops något hände när värderingen skulle tas bort");
      }
      catch (Exception ex)
      {
        return StatusCode(500, ex.Message);
      }
    }
  }
}