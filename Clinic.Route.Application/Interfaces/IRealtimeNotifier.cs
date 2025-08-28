using Clinic.Route.Contracts;
using Clinic.Route.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic.Route.Application.Interfaces
{
    public interface IRealtimeNotifier
    {
        // 🚨 Cambiamos la firma del método para recibir el DTO anidado
        Task NotifyExamenesActualizadosAsync(string groupKey, IEnumerable<ExamenPacienteDto> examenes);
    }
}
