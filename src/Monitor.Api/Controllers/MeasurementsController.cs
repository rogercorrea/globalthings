using Microsoft.AspNetCore.Mvc;
using Monitor.Api.DTOs;
using Monitor.Api.Services;

[ApiController]
[Route("api/[controller]")]
public class MeasurementsController : ControllerBase
{
    private readonly MeasurementService _svc;
    public MeasurementsController(MeasurementService svc) => _svc = svc;

    [HttpPost("batch")]
    public async Task<IActionResult> PostBatch([FromBody] MeasurementBatchDto dto)
    {
        if (dto?.Measurements == null || !dto.Measurements.Any())
            return BadRequest("No measurements provided.");

        await _svc.ProcessBatchAsync(dto.Measurements);
        return Accepted();
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] MeasurementDto dto)
    {
        await _svc.ProcessSingleAsync(dto);
        return Accepted();
    }
}
