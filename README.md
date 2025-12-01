# Respostas ao exercício técnico — Sensor Monitoring (DEV Pleno)

Este documento responde às questões do teste técnico, descreve as APIs implementadas no projeto e traz exemplos de código (C# / ASP.NET Core) usados no scaffold.

---

## PARTE 01

### a) Qual/Quais API(s) você criaria para receber os dados dos sensores? Justifique.

**APIs propostas (implementadas):**

1. `POST /api/measurements`  
   - Recebe uma única medição (MeasurementDto). Ideal para envio pontual.
2. `POST /api/measurements/batch`  
   - Recebe um lote de medições (MeasurementBatchDto). Ideal para firmware que acumula leituras e envia em lote quando a rede volta. Melhor desempenho e menor overhead por chamada.
3. Endpoints auxiliares de cadastro e consulta:
   - `GET /api/equipments/{id}/last-measurements` — consulta últimas medições por sensor (usado na Parte 02).
   - `POST /api/sensors/link` — vincular sensor ao equipamento/setor.

**Justificativa:**
- Separar *single* e *batch* permite que dispositivos com conectividade intermitente façam retry e enviem pacotes agregados sem criar overhead no servidor.
- Endpoints REST simples facilitam integração com firmware e integrações (Web UI, suporte).
- Uso de POST para inserção garante idempotência no cliente (com tratamento opcional de deduplicação no servidor).

---

### b) Como definir o objeto de request no ASP.NET Core? (exemplos)

**DTOs usados no projeto:**

`src/Monitor.Api/DTOs/MeasurementDto.cs`
```csharp
public record MeasurementDto
{
    public string Codigo { get; init; } = default!;
    public DateTimeOffset DataHoraMedicao { get; init; }
    public decimal Medicao { get; init; }
}
