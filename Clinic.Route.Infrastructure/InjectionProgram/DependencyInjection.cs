using Clinic.Route.Application.Features.Examenes.Queries;
using Clinic.Route.Application.Interfaces;
using Clinic.Route.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic.Route.Infrastructure.InjectionProgram
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            // Repositorios
            services.AddScoped<ISiglaRepository, SiglaRepository>();
            services.AddScoped<IExamenRepository, ExamenRepository>();
            services.AddScoped<GetExamenesPacienteQuery>();
            return services;
        }
    }
}
