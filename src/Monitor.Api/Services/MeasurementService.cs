using Monitor.Api.DTOs;
using Monitor.Api.Models;

namespace Monitor.Api.Services;

public class MeasurementService
{
    private readonly IMeasurementRepository _repo;
    private readonly AlertEvaluator _evaluator;

    public MeasurementService(IMeasurementRepository repo, AlertEvaluator evaluator)
    {
        _repo = repo;
        _evaluator = evaluator;
    }

    public async Task ProcessBatchAsync(IEnumerable<MeasurementDto> dtos)
    {
        var entities = dtos.Select(d => new Measurement
        {
            Codigo = d.Codigo,
            DataHoraMedicao = d.DataHoraMedicao,
            Medicao = d.Medicao
        }).ToList();

        await _repo.AddRangeAsync(entities);

        // avaliar alertas por sensor
        var grouped = entities.GroupBy(e => e.Codigo);
        foreach (var g in grouped)
        {
            await _evaluator.EvaluateAsync(g.Key);
        }
    }

    public async Task ProcessSingleAsync(MeasurementDto dto)
    {
        await ProcessBatchAsync(new[] { dto });
    }
}
