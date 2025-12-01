namespace Monitor.Api.Models;

public class Sensor
{
    public long Id { get; set; }
    public string Codigo { get; set; } = null!;
    public long? EquipmentId { get; set; }
    public Equipment? Equipment { get; set; }
}
