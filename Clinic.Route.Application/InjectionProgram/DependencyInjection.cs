
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Clinic.Route.Application.InjectionProgram
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Registra todos los handlers automáticamente usando MediatR 12
            services.AddMediatR(Assembly.GetExecutingAssembly());
            return services;
        }
    }
}
