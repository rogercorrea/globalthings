using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Xunit;

public class AlertEvaluatorTests
{
    [Fact]
    public async Task FiveConsecutiveBelow_ShouldNotify()
    {
        var repoMock = new Mock<IMeasurementRepository>();
        var notifierMock = new Mock<IAlertNotifier>();

        var measurements = Enumerable.Range(1, 50).Select(i => new Measurement
        {
            Codigo = "S1",
            DataHoraMedicao = DateTimeOffset.UtcNow.AddSeconds(-i),
            Medicao = i <= 5 ? 0.5m : 10m
        }).OrderByDescending(m => m.DataHoraMedicao).ToList();

        repoMock.Setup(r => r.GetLastMeasurementsAsync("S1", 50, default)).ReturnsAsync(measurements);

        var evaluator = new AlertEvaluator(repoMock.Object, notifierMock.Object);
        await evaluator.EvaluateAsync("S1");

        notifierMock.Verify(n => n.NotifyAsync("S1", It.IsAny<string?>(), It.Is<string>(s => s.Contains("5 medições")), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task FiveConsecutiveAbove_ShouldNotify()
    {
        var repoMock = new Mock<IMeasurementRepository>();
        var notifierMock = new Mock<IAlertNotifier>();

        var measurements = Enumerable.Range(1, 50).Select(i => new Measurement
        {
            Codigo = "S2",
            DataHoraMedicao = DateTimeOffset.UtcNow.AddSeconds(-i),
            Medicao = i <= 5 ? 100m : 10m
        }).OrderByDescending(m => m.DataHoraMedicao).ToList();

        repoMock.Setup(r => r.GetLastMeasurementsAsync("S2", 50, default)).ReturnsAsync(measurements);

        var evaluator = new AlertEvaluator(repoMock.Object, notifierMock.Object);
        await evaluator.EvaluateAsync("S2");

        notifierMock.Verify(n => n.NotifyAsync("S2", It.IsAny<string?>(), It.Is<string>(s => s.Contains("5 medições")), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task AverageOf50WithinMargin_ShouldNotify()
    {
        var repoMock = new Mock<IMeasurementRepository>();
        var notifierMock = new Mock<IAlertNotifier>();

        var measurements = Enumerable.Repeat(3m, 50).Select((val, idx) => new Measurement
        {
            Codigo = "S3",
            DataHoraMedicao = DateTimeOffset.UtcNow.AddSeconds(-idx),
            Medicao = val
        }).ToList();

        repoMock.Setup(r => r.GetLastMeasurementsAsync("S3", 50, default)).ReturnsAsync(measurements);

        var evaluator = new AlertEvaluator(repoMock.Object, notifierMock.Object);
        await evaluator.EvaluateAsync("S3");

        notifierMock.Verify(n => n.NotifyAsync("S3", It.IsAny<string?>(), It.Is<string>(s => s.Contains("média")), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task NoConditions_ShouldNotNotify()
    {
        var repoMock = new Mock<IMeasurementRepository>();
        var notifierMock = new Mock<IAlertNotifier>();

        var rnd = new Random();
        var measurements = Enumerable.Range(1, 50).Select(i => new Measurement
        {
            Codigo = "S4",
            DataHoraMedicao = DateTimeOffset.UtcNow.AddSeconds(-i),
            Medicao = (decimal)(10 + rnd.NextDouble() * 10)
        }).OrderByDescending(m => m.DataHoraMedicao).ToList();

        repoMock.Setup(r => r.GetLastMeasurementsAsync("S4", 50, default)).ReturnsAsync(measurements);

        var evaluator = new AlertEvaluator(repoMock.Object, notifierMock.Object);
        await evaluator.EvaluateAsync("S4");

        notifierMock.Verify(n => n.NotifyAsync(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}
