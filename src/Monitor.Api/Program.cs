using Microsoft.EntityFrameworkCore;
using Monitor.Api.Data;
using Monitor.Api.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(opts => {
    opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

var connStr = builder.Configuration.GetConnectionString("DefaultConnection") ??
              builder.Configuration["ConnectionStrings:DefaultConnection"];

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connStr));

// Serviços
builder.Services.AddScoped<IMeasurementRepository, EfMeasurementRepository>();
builder.Services.AddScoped<IAlertNotifier, EmailNotifier>();
builder.Services.AddScoped<AlertEvaluator>();
builder.Services.AddScoped<MeasurementService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // migrations automatizadas (desenvolvimento)
    db.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();
