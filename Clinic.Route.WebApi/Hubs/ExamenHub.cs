using Clinic.Route.Application.Service;
using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;
using static Clinic.Route.Application.Helpers.ExamenKeys;

namespace Clinic.Route.WebApi.Hubs
{
    public class ExamenHub : Hub
    {
       // 🔹 El cliente se une al grupo de la orden
        public async Task<bool> JoinOrderGroup(int codEmp, int codSed, int codTCl, int numOrd)
        {
            var group = GroupKeyBuilder.Build(codEmp, codSed, codTCl, numOrd);
            await Groups.AddToGroupAsync(Context.ConnectionId, group);
            return true; // 🔹 El front puede validar esto
        }

        // 🔹 El cliente deja de escuchar actualizaciones
        public async Task<bool> LeaveOrderGroup(int codEmp, int codSed, int codTCl, int numOrd)
        {
            var group = GroupKeyBuilder.Build(codEmp, codSed, codTCl, numOrd);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
            return true;
        }

        public override Task OnConnectedAsync()
        {
            Console.WriteLine($"✅ Cliente conectado: {Context.ConnectionId}");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"❌ Cliente desconectado: {Context.ConnectionId}, Error: {exception?.Message}");
            return base.OnDisconnectedAsync(exception);
        }

    }
}
