namespace Monitor.Api.Models;

public class Equipment
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<Sensor> Sensors { get; set; } = new List<Sensor>();
}
