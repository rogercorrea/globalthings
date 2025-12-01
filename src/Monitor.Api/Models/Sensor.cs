namespace Monitor.Api.Models;

public class Sensor
{
    public int Id { get; set; }
    public required string Codigo { get; set; }
    public long EquipmentId { get; set; }
    public Equipment? Equipment { get; set; }
}
