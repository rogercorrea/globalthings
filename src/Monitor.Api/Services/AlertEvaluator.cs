using Monitor.Api.Models;

public class AlertEvaluator
{
    private readonly IMeasurementRepository _repo;
    private readonly IAlertNotifier _notifier;
    private readonly decimal _lowerLimit = 1m;
    private readonly decimal _upperLimit = 50m;
    private readonly decimal _margin = 2m;

    public AlertEvaluator(IMeasurementRepository repo, IAlertNotifier notifier)
    {
        _repo = repo;
        _notifier = notifier;
    }

    public async Task EvaluateAsync(string sensorCode)
    {
        var last50 = (await _repo.GetLastMeasurementsAsync(sensorCode, 50)).ToList();
        if (!last50.Any()) return;

        var recent5 = last50.Take(5).ToList();
        if (recent5.Count == 5)
        {
            bool allBelow = recent5.All(m => m.Medicao < _lowerLimit);
            bool allAbove = recent5.All(m => m.Medicao > _upperLimit);
            if (allBelow || allAbove)
            {
                var subject = $"Alerta: sensor {sensorCode} com 5 medições consecutivas {(allBelow ? "abaixo" : "acima")} do limite";
                var body = $"Últimos 5: {string.Join(", ", recent5.Select(r => r.Medicao))}";
                await _notifier.NotifyAsync(sensorCode, null, subject, body);
                return;
            }
        }

        if (last50.Count >= 50)
        {
            var avg = last50.Average(m => m.Medicao);
            bool nearLower = avg >= (_lowerLimit - _margin) && avg <= (_lowerLimit + _margin);
            bool nearUpper = avg >= (_upperLimit - _margin) && avg <= (_upperLimit + _margin);
            if (nearLower || nearUpper)
            {
                var which = nearLower ? "inferior" : "superior";
                var subject = $"Atenção: média das últimas 50 medições do sensor {sensorCode} está próxima do limite {which}";
                var body = $"Média: {avg:F3}.";
                await _notifier.NotifyAsync(sensorCode, null, subject, body);
            }
        }
    }
}
