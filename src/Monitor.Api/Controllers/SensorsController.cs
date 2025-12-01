using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Monitor.Api.Data;
using Monitor.Api.Models;

[ApiController]
[Route("api/[controller]")]
public class SensorsController : ControllerBase
{
    private readonly AppDbContext _db;
    public SensorsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _db.Sensors.ToListAsync();
        return Ok(data);
    }

    [HttpPost("link")]
    public async Task<IActionResult> LinkSensor([FromBody] LinkSensorDto dto)
    {
        var sensor = await _db.Sensors.FindAsync(dto.SensorId);
        if (sensor == null) return NotFound("Sensor not found.");

        var eq = await _db.Equipments.FindAsync(dto.EquipmentId);
        if (eq == null) return NotFound("Equipment not found.");

        sensor.EquipmentId = dto.EquipmentId;
        await _db.SaveChangesAsync();
        return Ok();
    }
[HttpPost]
public async Task<IActionResult> Create([FromBody] CreateSensorDto dto)
{
    var equipment = await _db.Equipments.FindAsync(dto.EquipmentId);
    if (equipment == null)
        return NotFound("Equipment not found.");

    var sensor = new Sensor
    {
        Codigo = dto.Codigo,
        EquipmentId = dto.EquipmentId
    };

    _db.Sensors.Add(sensor);
    await _db.SaveChangesAsync();

    return CreatedAtAction(nameof(GetById), new { id = sensor.Id }, sensor);
}

[HttpGet("{id}")]
public async Task<IActionResult> GetById(int id)
{
    var sensor = await _db.Sensors.FindAsync(id);
    if (sensor == null) return NotFound();
    return Ok(sensor);
}
}
