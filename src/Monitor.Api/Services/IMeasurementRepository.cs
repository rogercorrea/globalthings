using Monitor.Api.Models;

public interface IMeasurementRepository
{
    Task AddRangeAsync(IEnumerable<Measurement> measurements, CancellationToken ct = default);
    Task<IReadOnlyList<Measurement>> GetLastMeasurementsAsync(string codigo, int count, CancellationToken ct = default);
}
