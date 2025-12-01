using Microsoft.AspNetCore.Mvc;
using Monitor.Api.Data;
using Monitor.Api.DTOs;
using Monitor.Api.Models;

[ApiController]
[Route("api/[controller]")]
public class SensorsController : ControllerBase
{
    private readonly AppDbContext _db;
    public SensorsController(AppDbContext db) => _db = db;

    [HttpPost("link")]
    public async Task<IActionResult> LinkSensor([FromBody] LinkSensorDto dto)
    {
        var sensor = await _db.Sensors.FindAsync(dto.SensorId);
        if (sensor == null) return NotFound("Sensor não encontrado.");

        var eq = await _db.Equipments.FindAsync(dto.EquipmentId);
        if (eq == null) return NotFound("Equipamento/Setor não encontrado.");

        sensor.EquipmentId = dto.EquipmentId;
        await _db.SaveChangesAsync();
        return Ok();
    }
}
