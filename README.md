# Sensor Monitoring API

Este projeto implementa uma aplicação para monitoramento de sensores conectados a equipamentos/setores, com armazenamento de medições em TimescaleDB e alertas via e-mail.

---

## PARTE 01 – APIs para receber medições

**a) Qual/Quais API(s) você criaria para que receber esses dados?**

Eu criaria a API:

```
POST /api/measurements
```

Justificativa: Um endpoint único para gravação de medições é suficiente, pois cada medição possui informações como `SensorId`, `Value` e `Timestamp`. Isso simplifica a ingestão de dados e facilita a integração com Kafka ou outros sistemas de mensageria.

**b) Objeto de request da API em ASP.NET Core**

```csharp
public class CreateMeasurementDto
{
    public long SensorId { get; set; }
    public double Value { get; set; }
    public DateTime Timestamp { get; set; }
}

[HttpPost]
[Route("api/measurements")]
public IActionResult Create([FromBody] CreateMeasurementDto dto)
{
    _measurementService.Add(dto);
    return Ok();
}
```

**c) Banco de dados recomendado**

TimescaleDB (PostgreSQL com extensão para séries temporais).

Justificativa: Otimizado para grandes volumes de dados por segundo, consultas por intervalos de tempo e agregações em séries temporais.
Também é possível utilizar bancos de dados puramente temporais, como InfluxDB, TSDB ou soluções pagas, com o Pi System, da Aveva.

---

## PARTE 02 – APIs para vincular sensores a equipamentos

**a) API para vincular Sensor a Equipamento/Setor**

```csharp
public class LinkSensorDto
{
    public long SensorId { get; set; }
    public long EquipmentId { get; set; }
}

[HttpPost("api/sensors/link")]
public IActionResult LinkSensor([FromBody] LinkSensorDto dto)
{
    _sensorService.LinkToEquipment(dto.SensorId, dto.EquipmentId);
    return Ok();
}
```

**b) API para retornar últimas 10 medições de cada sensor de um Equipamento/Setor**

```csharp
[HttpGet("api/equipments/{equipmentId}/measurements")]
public IActionResult GetLastMeasurements(long equipmentId)
{
    var sensors = _sensorService.GetByEquipmentId(equipmentId);
    var result = sensors.ToDictionary(
        s => s.Id,
        s => _measurementService.GetLastMeasurements(s.Id, 10)
    );
    return Ok(result);
}
```

---

## PARTE 03 – Alertas de medições fora de limites

**a) Proposta de solução**

- Usar **Kafka** para ingestão em tempo real.  
- Implementar regras no backend:  
  - 5 medições consecutivas fora do limite (1-50).  
  - Média das últimas 50 medições considerando margem de erro ±2.  
- Enviar alertas via e-mail (SMTP/MailHog para testes).  

**b) Algoritmo de alerta**

```csharp
public void EvaluateSensor(Sensor sensor)
{
    var last50 = _measurementService.GetLastMeasurements(sensor.Id, 50);
    double average = last50.Average(m => m.Value);

    // Regra 1: 5 medições consecutivas fora do limite
    if (last50.TakeLast(5).All(m => m.Value < 1 || m.Value > 50))
        _alertNotifier.Send($"Sensor {sensor.Id} excedeu limites 5 vezes consecutivas");

    // Regra 2: Média com margem de erro
    if (average < 1 + 2 || average > 50 - 2)
        _alertNotifier.Send($"Sensor {sensor.Id} merece atenção (média = {average})");
}
```

**c) Exemplo de teste unitário**

```csharp
[Fact]
public void Alert_When_5ConsecutiveOutOfRange()
{
    var measurements = new List<Measurement>
    {
        new Measurement { Value = 0.5 },
        new Measurement { Value = 0.8 },
        new Measurement { Value = 51 },
        new Measurement { Value = 52 },
        new Measurement { Value = 49 }
    };

    var sensor = new Sensor { Id = 1 };
    _measurementServiceMock.Setup(m => m.GetLastMeasurements(1, 50))
        .Returns(measurements);

    _alertEvaluator.EvaluateSensor(sensor);

    _alertNotifierMock.Verify(n => n.Send(It.IsAny<string>()), Times.AtLeastOnce);
}
```

---

## PARTE 04 – Alta ingestão de dados

**Solução proposta**

- Ingestão assíncrona via **Kafka**.  
- Escalar **consumers** em paralelo para processar dados.  
- Particionar tópicos por sensor/setor para paralelismo.  
- Retenção e compressão de dados em **TimescaleDB** para otimizar armazenamento.  

Justificativa: Desacopla ingestão e processamento, garantindo que picos de dados não travem o sistema.

---

## Tecnologias utilizadas

- **ASP.NET Core 7** – Backend e APIs REST  
- **Entity Framework Core** – ORM para PostgreSQL/TimescaleDB  
- **TimescaleDB** – Banco de dados de séries temporais  
- **Kafka** – Mensageria em tempo real  
- **MailHog** – Testes de envio de e-mail  
- **Swagger** – Documentação e testes de API  

---

## Estrutura de pastas

```
/src
    /Controllers
    /Data
    /Services
/Dockerfile
/docker-compose.yml
/README.md
```

---

## Como rodar o projeto

```bash
docker compose up --build
```

- API: `http://localhost:8080/swagger`
- MailHog: `http://localhost:8025`
- TimescaleDB: porta 5432
- Kafka: porta 9092

