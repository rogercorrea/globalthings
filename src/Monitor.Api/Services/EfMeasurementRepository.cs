using Microsoft.EntityFrameworkCore;
using Monitor.Api.Data;
using Monitor.Api.Models;

public class EfMeasurementRepository : IMeasurementRepository
{
    private readonly AppDbContext _db;
    public EfMeasurementRepository(AppDbContext db) => _db = db;

    public async Task AddRangeAsync(IEnumerable<Measurement> measurements, CancellationToken ct = default)
    {
        _db.Measurements.AddRange(measurements);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<Measurement>> GetLastMeasurementsAsync(string codigo, int count, CancellationToken ct = default)
    {
        return await _db.Measurements
            .Where(m => m.Codigo == codigo)
            .OrderByDescending(m => m.DataHoraMedicao)
            .Take(count)
            .ToListAsync(ct);
    }
}
