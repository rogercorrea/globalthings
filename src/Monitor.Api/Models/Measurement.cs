namespace Monitor.Api.Models;

public class Measurement
{
    public long Id { get; set; }
    public string Codigo { get; set; } = null!;
    public DateTimeOffset DataHoraMedicao { get; set; }
    public decimal Medicao { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
