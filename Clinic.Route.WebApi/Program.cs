using Clinic.Route.Application.Features.Examenes.Queries;
using Clinic.Route.Application.InjectionProgram;
using Clinic.Route.Application.Interfaces;
using Clinic.Route.Infrastructure.InjectionProgram;
using Clinic.Route.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);


// 🔹 Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularClient", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // dominio de Angular
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
////REPOSITORIOS
//builder.Services.AddScoped<ISiglaRepository, SiglaRepository>();
//// Registro de casos de uso
//builder.Services.AddScoped<GetExamenesPacienteQuery>();

builder.Services.AddApplication();
builder.Services.AddInfrastructure();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 🔹 Usar CORS ANTES de MapControllers
app.UseCors("AllowAngularClient");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
