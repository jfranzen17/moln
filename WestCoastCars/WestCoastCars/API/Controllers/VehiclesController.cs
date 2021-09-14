using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Interfaces;
using API.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
  [ApiController]
  [Route("api/vehicles")]
  public class VehiclesController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;

    public VehiclesController(IUnitOfWork unitOfWork)
    {
      _unitOfWork = unitOfWork;
    }

    [HttpGet()]
    public async Task<ActionResult<IEnumerable<VehicleViewModel>>> GetVehicles()
    {
      return Ok(await _unitOfWork.VehicleRepository.GetVehiclesAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<VehicleViewModel>> GetVehicle(int id)
    {
      var result = await _unitOfWork.VehicleRepository.GetVehicleByIdAsync(id);
      if (result == null) return NotFound();
      return Ok(result);
    }

    [HttpGet("find/{regNo}")]
    public async Task<ActionResult<VehicleViewModel>> FindVehicle(string regNo)
    {
      var result = await _unitOfWork.VehicleRepository.GetVehicleByRegNoAsync(regNo);
      if (result == null) return NotFound();
      return Ok(result);
    }

    [HttpPost()]
    public async Task<ActionResult> AddVehicle(AddVehicleViewModel model)
    {
      try
      {
        var manufacturer = await _unitOfWork.manufacturerRepository.GetManufacturerByName(model.Make);
        if (manufacturer == null) return BadRequest($"Tillverkaren {model.Make} finns ej i systemet");
        var vehicleModel = await _unitOfWork.VehicleModelRepository.GetModelByName(model.Model);
        if (vehicleModel == null) return BadRequest($"Model beteckning {model.Model} finns ej i systemet");

        _unitOfWork.VehicleRepository.Add(model);
        if (await _unitOfWork.Complete())
        {
          return StatusCode(201);
        }
        return StatusCode(500, "Gick inte att spara fordonet");
      }
      catch (Exception ex)
      {
        return StatusCode(500, ex.Message);
      }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteVehicle(int id)
    {
      try
      {
        var vehicle = await _unitOfWork.VehicleRepository.GetVehicleByIdAsync(id);
        if (vehicle == null) return NotFound($"Tyvärr hittade ingen bil med id {id}");

        _unitOfWork.VehicleRepository.Delete(vehicle);
        if (await _unitOfWork.Complete()) return NoContent();

        return StatusCode(500, "Gick inte att ta bort fordonet");
      }
      catch (Exception ex)
      {
        return StatusCode(500, ex.Message);
      }
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult> UpdateVehicle(int id, UpdateVehicleViewModel model)
    {
      try
      {
        var vehicle = await _unitOfWork.VehicleRepository.GetVehicleByIdAsync(id);

        vehicle.FuelType = model.FuelType;
        vehicle.GearType = model.GearType;
        vehicle.Mileage = model.Mileage;
        vehicle.RegistrationDate = model.RegistrationDate;

        _unitOfWork.VehicleRepository.Update(vehicle);

        if (await _unitOfWork.Complete()) return NoContent();

        return StatusCode(500, "Det gick inte att uppdatera fordonet");
      }
      catch (Exception ex)
      {
        return StatusCode(500, ex.Message);
      }
    }
  }
}