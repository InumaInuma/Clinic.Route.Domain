using Clinic.Route.Application.Service;
using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;
using static Clinic.Route.Application.Helpers.ExamenKeys;

namespace Clinic.Route.WebApi.Hubs
{
    public class ExamenHub : Hub
    {

        // El cliente envía los parámetros y se une a su grupo (una orden = un grupo)
        public Task JoinOrderGroup(int codEmp, int codSed, int codTCl, int numOrd)
        {
            // Usamos GroupKeyBuilder en lugar de ExamenService
            var group = GroupKeyBuilder.Build(codEmp, codSed, codTCl, numOrd);
            return Groups.AddToGroupAsync(Context.ConnectionId, group);
        }

        public Task LeaveOrderGroup(int codEmp, int codSed, int codTCl, int numOrd)
        {
            var group = GroupKeyBuilder.Build(codEmp, codSed, codTCl, numOrd);
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
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
