public interface IAlertNotifier
{
    Task NotifyAsync(string sensorCode, string? equipmentInfo, string subject, string body);
}
