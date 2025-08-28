using Clinic.Route.Application.Interfaces;
using Clinic.Route.Contracts;
using Clinic.Route.Domain.Entities;
using Clinic.Route.WebApi.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Clinic.Route.WebApi.Realtime
{
    public class SignalRNotifier : IRealtimeNotifier
    {
        private readonly IHubContext<ExamenHub> _hub;
        private readonly ILogger<SignalRNotifier> _logger; // 🔹 Logger

        public SignalRNotifier(IHubContext<ExamenHub> hub, ILogger<SignalRNotifier> logger)
        {
            _hub = hub;
            _logger = logger;
        }


        //public Task NotifyExamenesActualizadosAsync(string groupKey, IEnumerable<ExamenPaciente> examenes)
        //    => _hub.Clients.Group(groupKey).SendAsync("ExamenesActualizados", examenes);
        public async Task NotifyExamenesActualizadosAsync(string groupKey, IEnumerable<ExamenPacienteDto> examenes)
        {
            if (examenes == null)
            {
                _logger.LogWarning("⚠️ Se recibió null en examenes para el grupo {GroupKey}", groupKey);
                return;
            }

            _logger.LogInformation("📥 Exámenes recibidos antes de mapear: {Count}", examenes.Count());
            _logger.LogInformation("📥 Datos crudos: {@Examenes}", examenes);

            _logger.LogInformation("📤 Enviando exámenes: {@Data}", examenes);
            await _hub.Clients.Group(groupKey).SendAsync("ExamenesActualizados", examenes);
            _logger.LogInformation("📡 Notificación enviada al grupo {GroupKey} con {Count} exámenes", groupKey, examenes?.Count() ?? 0);
        }
    }
}
