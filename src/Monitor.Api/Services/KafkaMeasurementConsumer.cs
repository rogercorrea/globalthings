using Confluent.Kafka;
using System.Text.Json;
using Monitor.Api.Data;
using Monitor.Api.Models;

namespace Monitor.Api.Services;

public class KafkaMeasurementConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _config;

    public KafkaMeasurementConsumer(IServiceProvider serviceProvider, IConfiguration config)
    {
        _serviceProvider = serviceProvider;
        _config = config;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var topic = _config["Kafka:Topic"] ?? "sensors.measurements";

        var config = new ConsumerConfig
        {
            BootstrapServers = _config["Kafka:BootstrapServers"] ?? "kafka:9092",
            GroupId = "sensor-api-consumer",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();

        consumer.Subscribe(topic);
        Console.WriteLine($"[KafkaConsumer] Listening topic: {topic}");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var cr = consumer.Consume(stoppingToken);

                Console.WriteLine($"[KafkaConsumer] Received: {cr.Message.Value}");

                var measurement = JsonSerializer.Deserialize<Measurement>(cr.Message.Value);

                if (measurement != null)
                {
                    using var scope = _serviceProvider.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    db.Measurements.Add(measurement);
                    await db.SaveChangesAsync(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[KafkaConsumer] Error: {ex.Message}");
            }
        }
    }
}
