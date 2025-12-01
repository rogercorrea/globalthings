using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Monitor.Api.Data;
using Monitor.Api.DTOs;

[ApiController]
[Route("api/[controller]")]
public class EquipmentsController : ControllerBase
{
    private readonly AppDbContext _db;
    public EquipmentsController(AppDbContext db) => _db = db;

    [HttpGet("{id}/last-measurements")]
    public async Task<IActionResult> GetLastMeasurements(long id)
    {
        var sensors = await _db.Sensors
            .Where(s => s.EquipmentId == id)
            .Select(s => s.Codigo)
            .ToListAsync();

        if (!sensors.Any()) return NotFound("Nenhum sensor vinculado ao equipamento.");

        // query única para otimizar: usar window function via raw SQL
        var codes = string.Join(',', sensors.Select(c => "'" + c + "'"));

        var sql = $@"
            SELECT Codigo, DataHoraMedicao, Medicao FROM (
              SELECT m.*, ROW_NUMBER() OVER (PARTITION BY Codigo ORDER BY DataHoraMedicao DESC) rn
              FROM Measurements m
              WHERE Codigo IN ({codes})
            ) t
            WHERE rn <= 10
            ORDER BY Codigo, DataHoraMedicao DESC;
        ";

        var list = await _db.Measurements.FromSqlRaw(sql).ToListAsync();

        var grouped = list.GroupBy(m => m.Codigo)
            .ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.DataHoraMedicao).Select(m => new MeasurementDto {
                Codigo = m.Codigo,
                DataHoraMedicao = m.DataHoraMedicao,
                Medicao = m.Medicao
            }).ToList());

        return Ok(grouped);
    }
}
