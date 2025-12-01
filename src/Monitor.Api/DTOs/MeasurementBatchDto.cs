namespace Monitor.Api.DTOs;

public record MeasurementBatchDto
{
    public string? DeviceId { get; init; }
    public IReadOnlyList<MeasurementDto> Measurements { get; init; } = Array.Empty<MeasurementDto>();
}
