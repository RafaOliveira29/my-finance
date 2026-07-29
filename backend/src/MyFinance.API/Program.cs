using MyFinance.API.Middleware;
using MyFinance.Application;
using MyFinance.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Erro padronizado → ProblemDetails (CA071 / HT05): nenhum 500 cru, nenhum erro silencioso.
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Camadas (Clean Architecture pragmática).
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // /openapi/v1.json → base para gerar os tipos TS do front (CA073).
}

app.UseHttpsRedirection();
app.MapControllers();

// Healthcheck raiz (Fase 0) — smoke test de que a API sobe.
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();

/// <summary>Exposto para os testes de integração da API.</summary>
public partial class Program;
