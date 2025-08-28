using Clinic.Route.Application.Features.Examenes.Queries;
using Clinic.Route.Application.InjectionProgram;
using Clinic.Route.Application.Interfaces;
using Clinic.Route.Application.Service;
using Clinic.Route.Infrastructure.InjectionProgram;
using Clinic.Route.Infrastructure.Repositories;
using Clinic.Route.WebApi.Hubs;
using Clinic.Route.WebApi.Realtime;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);


// 🔐 CORS (ajusta origen al de tu Angular)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        p => p.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .SetIsOriginAllowed(_ => true)); // 👈 habilita todos los orígenes en dev);

});

// Add services to the container.

//builder.Services.AddControllers();
// 🔧 Controllers + JSON camelCase (para que Angular reciba codPer, nomPer, etc.)
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        o.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    });

// 📡 SignalR
builder.Services.AddSignalR();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
////REPOSITORIOS
//builder.Services.AddScoped<ISiglaRepository, SiglaRepository>();
//// Registro de casos de uso
//builder.Services.AddScoped<GetExamenesPacienteQuery>();

builder.Services.AddApplication();
builder.Services.AddInfrastructure();

// 🧩 DI: repos, servicios y notifier
builder.Services.AddScoped<IExamenRepository, ExamenRepository>();
builder.Services.AddScoped<IExamenService, ExamenService>();
builder.Services.AddSingleton<IRealtimeNotifier, SignalRNotifier>();
//builder.Services.AddScoped<IRealtimeNotifier, SignalRNotifier>();
//builder.Services.AddScoped<IExamenService, ExamenService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // 👉 Esto te muestra el stacktrace completo
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 🔹 Usar CORS ANTES de MapControllers
app.UseCors("AllowAll");

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// RUTA DEL HUB (debe coincidir con tu Angular: /hubs/examenes)
app.MapHub<ExamenHub>("/hubs/examenes");

app.Run();
