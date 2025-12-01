namespace Monitor.Api.DTOs;

public record MeasurementDto
{
    public string Codigo { get; init; } = default!;
    public DateTimeOffset DataHoraMedicao { get; init; }
    public decimal Medicao { get; init; }
}
